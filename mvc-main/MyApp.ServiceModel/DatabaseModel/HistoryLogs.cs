using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ServiceModel.DatabaseModel
{
    public class HistoryLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string LogType { get; set; }

        public string LogMessage { get; set; }

        public string QuoteModelId { get; set; }

        public DateTime LogDate { get; set; } = DateTime.Now;
    }

}
