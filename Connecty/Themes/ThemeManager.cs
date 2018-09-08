using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace Connecty
{
    /// <summary>
    /// Custom theme manager.
    /// </summary>
    class ThemeManager
    {
        /// <summary>
        /// Dictionary that holds all the resource dictionaries by theme name.
        /// </summary>
        private Dictionary<string, ResourceDictionary> themeDictionary = new Dictionary<string, ResourceDictionary>();

        /// <summary>
        /// The list of theme names
        /// </summary>
        public List<string> ThemeNameList { get; private set; }

        /// <summary>
        /// Gets the name of the current theme.
        /// </summary>
        public string CurrentTheme { get; private set; }

        /// <summary>
        /// Retrieves the list of themes from the assembly.
        /// </summary>
        /// <returns>The list of theme names.</returns>
        private List<string> RetrieveThemeList()
        {
            // Get the assembly that we are currently in.
            Assembly assembly = Assembly.GetEntryAssembly();

            // Get the name of the resources file that we will load.
            string resourceFileName = assembly.GetName().Name + ".g.resources";

            // Open the manifest stream.
            using (Stream stream = assembly.GetManifestResourceStream(resourceFileName))
            {
                // Open a resource reader to get all the resource files.
                using (ResourceReader reader = new ResourceReader(stream))
                {
                    // Returns just the resources that start in the themes folder
                    return new List<string>(reader.Cast<DictionaryEntry>()
                        .Where(entry => entry.Key.ToString().StartsWith("themes/")) // Get all files that are in the themes folder.
                        .Select(entry => entry.Key.ToString().Replace(".baml", "").Replace("themes/", "")) // select the name without .baml or themes/
                        .OrderBy(entry => entry.ToString()).ToArray()); // put it in alpha order.
                }
            }
        }

        /// <summary>
        /// Constructor that handles loading the theme list.
        /// </summary>
        public ThemeManager()
        {
            // Set the starting theme.
            CurrentTheme = String.Empty;

            // Load the theme list into memory.
            LoadThemes();
        }

        /// <summary>
        /// Loads the theme name list and theme dictionary.
        /// </summary>
        private void LoadThemes()
        {
            // Get the list of theme names from the method above.
            ThemeNameList = RetrieveThemeList();

            // Go through each theme in the name list.
            foreach (string theme in ThemeNameList)
            {
                // and load the theme dictionary.
                ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(new Uri(String.Format(@"Connecty;component\themes/{0}.xaml", theme), UriKind.Relative));

                // then add it to our theme dictionary.
                themeDictionary.Add(theme, resourceDictionary);
            }
        }


        /// <summary>
        /// This method handles removing the previous theme and setting the new one.
        /// </summary>
        /// <param name="themeName">The theme we want to use.</param>
        public void SetTheme(string themeName)
        {
            // If we currently have a theme.
            if (CurrentTheme != String.Empty)
                // Remove it from the merged dictionaries.
                Application.Current.Resources.MergedDictionaries.Remove(themeDictionary[CurrentTheme]);

            // If we have this theme.
            if (themeDictionary.ContainsKey(themeName))
                // Add our new theme to the merged dictionaries.
                Application.Current.Resources.MergedDictionaries.Add(themeDictionary[themeName]);

            // Set the current theme name.
            CurrentTheme = themeName;
        }
    }

}
