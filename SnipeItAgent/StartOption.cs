using System;
using CommandLine;

namespace SnipeItAgent
{
    public class StartOption
    {
        [Option('c', "config", HelpText = "Path to configuration file.", Default = "config.json")]
        public string ConfigPath { get; set; }
        
        [Option('u', "uri", HelpText = "Uri of your Snipe IT instance.")]
        public Uri Uri { get; set; }
        
        [Option('t', "apiToken", HelpText = "ApiToken to access your Snipe IT instance.")]
        public string ApiToken { get; set; }
        
        [Option('l', "location", HelpText = "Id of the location the asset should be assigned to.")]
        public int? LocationId { get; set; }
        
        [Option('s', "status", HelpText = "Id of the status label the asset should be assigned to.")]
        public int? StatusLabelId { get; set; }

        [Option('C', "company", HelpText = "Id of the company the asset should be assigned to.")]
        public int? CompanyId { get; set; }

        [Option('m', "manufacturer", HelpText = "Id of the manufacturer the asset should be assigned to.")]
        public int? ManufacturerId { get; set; }

        [Option('M', "model", HelpText = "Id of the model the asset should be assigned to.")]
        public int? ModelId { get; set; }

        [Option("client", HelpText = "Overrides detection whether this machine is a client or server machine.")]
        public bool? IsClient { get; set; }
        
        [Option("notebook", HelpText = "Overrides detection whether this machine is a notebook or desktop computer.")]
        public bool? IsNotebook { get; set; }
        
        [Option('a', "asset", HelpText = "Use this asset tag when creating the asset.")]
        public string NewAssetTag { get; set; }
        
        [Option('n', "name", HelpText = "Use this asset name when creating or updating the asset.")]
        public string AssetName { get; set; }
    }
}