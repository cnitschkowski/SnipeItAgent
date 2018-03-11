using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using SnipeSharp.Endpoints.Models;

namespace SnipeItAgent
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            int returnCode = -1;

            Parser.Default.ParseArguments<StartOption>(args)
                .WithParsed(options => returnCode = CreateOrUpdateAsset(options)).WithNotParsed(errors => returnCode = HandleErrors(errors));

            return returnCode;
        }

        private static int HandleErrors(IEnumerable<Error> errors)
        {
            return 2;
        }
        
        private static int CreateOrUpdateAsset(StartOption startOption)
        {
            var config = GetConfig(startOption);

            var access = new SnipeAccess
            {
                ApiToken = config.ApiToken,
                Uri = config.Uri
            };

            var provider = new WindowsSystemInfoProvider();
            
            var info = provider.GetSystemInfo();

            var isClient = startOption.IsClient ?? info.IsClient;
            var isNotebook = startOption.IsNotebook ?? info.IsNotebook;

            var manufacturer = (startOption.ManufacturerId.HasValue
                                   ? access.Get<Manufacturer>(startOption.ManufacturerId.Value)
                                   : access.Get<Manufacturer>(info.Manufacturer)) ??
                               access.Create(new Manufacturer {Name = info.Manufacturer});

            var model = startOption.ModelId.HasValue
                ? access.Get<Model>(startOption.ModelId.Value)
                : access.Get<Model>(info.Model);
            if (model == null)
            {
                var categoryName = isClient ? isNotebook ? "Notebook" : "Desktop" : "Server";
                var category =
                    access.Get<Category>(categoryName) ??
                    access.Create(new Category { Name = categoryName, Type = "asset"});

                model = new Model {Name = info.Model, Manufacturer = manufacturer, Category = category};
                model = access.Create(model);
            }

            var assetName = string.IsNullOrEmpty(startOption.AssetName) ? info.Hostname : startOption.AssetName;
            var asset = access.Get<Asset>(assetName) ?? access.Get<Asset>(info.Hostname);
            Func<Asset, Asset> saveAsset;
            
            if (asset == null)
            {
                StatusLabel statusLabel;
                if (config.StatusLabelId > 0)
                {
                    statusLabel = access.Get<StatusLabel>(config.StatusLabelId);
                }
                else
                {
                    var labels = access.Get<StatusLabel>();
                    statusLabel = labels.FirstOrDefault(l => l.Type == "deployable");
                }

                asset = new Asset
                {
                    Manufacturer = manufacturer,
                    Model = model,
                    Name = assetName,
                    Serial = info.SerialNumber,
                    AssetTag = startOption.NewAssetTag ?? string.Empty,
                    StatusLabel = statusLabel
                };

                saveAsset = a => access.Create(a);
            }
            else
            {
                saveAsset = a => access.Update(a);
            }

            // Update the company if not known yet or changed using options.
            if ((asset.Company == null || asset.Company.Id != config.CompanyId) && config.CompanyId > 0)
            {
                asset.Company = access.Get<Company>(config.CompanyId);
            }

            // Update the company if not known yet or changed.
            if ((asset.Location == null || asset.Location.Id != config.LocationId) && config.LocationId > 0)
            {
                asset.Location = access.Get<Location>(config.LocationId);
            }
            
            var customFields = asset.CustomFields;
            if (customFields == null)
            {
                customFields = new Dictionary<string, string>();
                asset.CustomFields = customFields;
            }
            
            customFields.AddOrSet("Memory", info.Memory.ToString());
            customFields.AddOrSet("Platform", info.Platform);

            asset = saveAsset(asset);

            if (asset == null)
            {
                return 1;
            }

            Console.Out.Write(asset.AssetTag);
            return 0;
        }

        private static Config GetConfig(StartOption startOption)
        {
            var configPath = Path.IsPathRooted(startOption.ConfigPath)
                ? startOption.ConfigPath
                : Path.Combine(Environment.CurrentDirectory, startOption.ConfigPath);

            Config config;
            if (File.Exists(configPath))
            {
                var configSource = new JsonConfigSource {FilePath = configPath};
                config = configSource.Read();
            }
            else
            {
                config = new Config();
            }

            if (startOption.Uri != null)
            {
                config.Uri = startOption.Uri;
            }

            if (!string.IsNullOrEmpty(startOption.ApiToken))
            {
                config.ApiToken = startOption.ApiToken;
            }

            if (startOption.CompanyId.HasValue)
            {
                config.CompanyId = startOption.CompanyId.Value;
            }

            if (startOption.LocationId.HasValue)
            {
                config.LocationId = startOption.LocationId.Value;
            }

            if (startOption.StatusLabelId.HasValue)
            {
                config.StatusLabelId = startOption.StatusLabelId.Value;
            }

            return config;
        }
    }
}