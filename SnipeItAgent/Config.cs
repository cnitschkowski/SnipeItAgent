
using System;

namespace SnipeItAgent
{
    public class Config
    {
        public Uri Uri { get; set; }
        
        public string ApiToken { get; set; }
        
        public long LocationId { get; set; }
        
        public long StatusLabelId { get; set; }
        
        public long CompanyId { get; set; }
    }
}