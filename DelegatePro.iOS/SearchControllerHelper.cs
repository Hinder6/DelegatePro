using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public static class SearchControllerHelper
    {
        public static UISearchController Create(IUISearchControllerDelegate searchControllerDelegate,  IUISearchBarDelegate searchBarDelegate)
        {
            var sc = new UISearchController(searchResultsController: null);

            var searchBar = sc.SearchBar;
            searchBar.BarTintColor = UIColor.FromRGB(242, 242, 242);

            searchBar.SizeToFit();
            searchBar.Delegate = searchBarDelegate;

            sc.Delegate = searchControllerDelegate;
            sc.DimsBackgroundDuringPresentation = false; // default is YES

            return sc;
        }
    }
}

