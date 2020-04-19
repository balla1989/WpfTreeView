using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.IO;

namespace WpfTreeView{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Construtor
        //Default Constructor
        public MainWindow()
        {
            InitializeComponent();  
            
        }
        #endregion

        #region On Loaded

        /// <summary>
        /// When the application first opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Get Every drive in the machine
            foreach (var drive in Directory.GetLogicalDrives())

            {                   
                //create a new item for it 
                TreeViewItem item = new TreeViewItem()                
                {
                    //Set the header 
                    Header = drive,

                    //and the full path
                    Tag = drive
                };              

                //Add a dummy item
                item.Items.Add(null);


                //Liste out for items being expanded
                item.Expanded += Folder_Expanded;
                

                //Add it to main tree view
                FolderView.Items.Add(item);

            }
        }

        #endregion

        #region FOLDER  EXPANDED


        /// <summary>
        /// When a folder is expanded,  find the subfolders/files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region INITIAL CHECKS
            var item = (TreeViewItem)sender;

            //If the item contains dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            // Clear dummy data
            item.Items.Clear();


            //Get full path
            var fullPath = (string)item.Tag;

            #endregion

            #region GET FOLDERS

            //Create a blank list for directories
            var directories = new List<string>();


            //Try and get directories from the folder
            //ignoring any issues doing so

            try
            {
                var dirs = Directory.GetDirectories(fullPath);
                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }

            catch
            {

            }

            //For each directory
            directories.ForEach(directoryPath =>
            {
                //Create directory item
                var subItem = new TreeViewItem()
                {
                    //Set Header as folder name
                    Header = GetFileFolderName(directoryPath),

                    //And tag as full path
                    Tag = directoryPath
                };

                

                // Add dummy item so we can expand folder
                subItem.Items.Add(null);

                // Handle expanding
                subItem.Expanded += Folder_Expanded;

                //Add this item to the parent
                item.Items.Add(subItem);
            });
            #endregion

            #region GET FILES
            //Create a blank list for FILES
            var files = new List<string>();

            try
            {
                var fs = Directory.GetFiles(fullPath);
                if (fs.Length > 0)
                    directories.AddRange(fs);
            }

            catch
            {

            }

            //For each files
            files.ForEach(filePath =>
            {
                //Create file item
                var subItem = new TreeViewItem()
                {
                    //Set Header as file name
                    Header = GetFileFolderName(filePath),

                    //And tag as full path
                    Tag = filePath
                };                                   

                //Add this item to the parent
                item.Items.Add(subItem);
            });
            #endregion
        }
        #endregion


        #region HELPER FUNCTION TO FIND FILE OR FOLDER NAME FROM FULL PATH
        /// <summary>
        /// 
        /// HELPER FUNCTION
        /// Finds the file or folder name from full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            //C:\something\folder
            // we want to get 'folder'

            //if we have no path, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            //Make all slashes back slashes
            var normalizedPath = path.Replace('/', '\\');


            //Find the lash backslahd in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            //If we don't find a backslash, return path itself
            if (lastIndex <= 0)
                return path;

            //return the name after last back slash
            return path.Substring(lastIndex + 1);
        }
        #endregion


    }
}
