using SitefinityWebApp.Custom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class ClassificationHelper
    {
        public const string EmployeeList = "employees";

        public static IEnumerable<Taxon> GetTaxaFromSimpleListClassification(string classificationName)
        {
            TaxonomyManager manager = TaxonomyManager.GetManager();
            FlatTaxonomy classification = manager.GetTaxonomies<FlatTaxonomy>().FirstOrDefault(t => t.Name == classificationName);
            if (classification == null)
                throw new Exception("Unable to find " + classificationName + " in Flat Taxonomies.");

            return classification.Taxa;
        }

        public static IEnumerable<Taxon> GetTaxaFromHeirarchicalListClassification(string classificationName)
        {
            TaxonomyManager manager = TaxonomyManager.GetManager();
            HierarchicalTaxonomy classification = manager.GetTaxonomies<HierarchicalTaxonomy>().FirstOrDefault(t => t.Name == classificationName);
            if (classification == null)
                throw new Exception("Unable to find " + classificationName + " in Heirarchical Taxonomies.");

            return classification.Taxa;
        }

        public static IEnumerable<Taxon> GetChildrenTaxaByParentName(string classificationName, string parentName)
        {
            return GetTaxaFromHeirarchicalListClassification(classificationName)
                .Where(x => x.Parent != null && x.Parent.Name.ToLower() == parentName.ToLower());
        }

        public static IEnumerable<Taxon> GetLastChildren(string classificationName)
        {
            List<Taxon> lastChildren = new List<Taxon>();
            IEnumerable<Taxon> taxa = GetTaxaFromHeirarchicalListClassification(classificationName);

            foreach (Taxon item in taxa)
            {
                bool hasChild = taxa.Any(x => x.Parent != null && x.Parent.Name == item.Name);
                if (!hasChild)
                    lastChildren.Add(item);
            }

            return lastChildren.OrderBy(x => x.Title).ToList();
        }

        public static Taxon GetParentTaxonByChildName(string classificationName, string childName)
        {
            Taxon child = GetTaxaFromHeirarchicalListClassification(classificationName).FirstOrDefault(x => x.Name == childName);
            if (child == null)
                throw new Exception("Classification item does not exist!");
            if (child.Parent == null)
                throw new Exception("This is a parent classification item!");
            return child.Parent;
        }

        public static IEnumerable<Taxon> GetAllCategoryByParent(string parentName)
        {

            var taxonomyMgr = TaxonomyManager.GetManager();
            IEnumerable<Taxon> taxon = null;
            TaxonomyManager manager = TaxonomyManager.GetManager();
            HierarchicalTaxonomy classification = manager.GetTaxonomies<HierarchicalTaxonomy>().FirstOrDefault(t => t.Name == "Categories");
            if (classification != null && !string.IsNullOrEmpty(parentName))
            {
                taxon = classification.Taxa.Where(x => x.Parent != null && x.Parent.Name == parentName);
            }
            return taxon;
        }
    }
}