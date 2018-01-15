using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Custom.Models
{
    public class TaxonModel
    {
        public TaxonModel()
        {
 
        }

        public TaxonModel(Taxon taxon)
        {
            Id = taxon.Id;
            Title = taxon.Title;
            SortOrder = GetSortOrder(taxon);
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public bool IsChecked { get; set; }

        private int GetSortOrder(Taxon taxon)
        {
            KeyValuePair<string, string> sortOrderAttribute = taxon.Attributes.FirstOrDefault(x => x.Key == "SortOrder");
            int sortOrder = 0;
            if (!sortOrderAttribute.Equals(default(KeyValuePair<string, string>)))
            {
                int.TryParse(sortOrderAttribute.Value, out sortOrder);
            }
            return sortOrder;
        }
    }
}