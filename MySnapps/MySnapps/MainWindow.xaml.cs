using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MySnapps.Data;
using MySnapps.Generator;
using MySnapps.Scheduler;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;

namespace MySnapps
{

    public partial class MainWindow
    {
        private bool _stopRefreshControls;
        private bool _dataChanged;
        private readonly BackgroundWorker _mOWorker;
        private CancellationTokenSource _tokenSource;

        public MainWindow()
        {
            InitializeComponent();

            _mOWorker = new BackgroundWorker();
            _mOWorker.DoWork += m_oWorker_DoWork;
            _mOWorker.ProgressChanged += m_oWorker_ProgressChanged;
            _mOWorker.RunWorkerCompleted += m_oWorker_RunWorkerCompleted;
            _mOWorker.WorkerReportsProgress = true;
            _mOWorker.WorkerSupportsCancellation = true;
        }

        #region DataEvent
        /// <summary>
        /// addButton click event handler.
        /// Add a new row to the ListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            SetDataChanged(true);
            AddRow();
        }

        /// <summary>
        /// Adds a blank row to the ListView
        /// </summary>
        private void AddRow()
        {
            urlListView.Items.Add(new ListViewData(""));
            urlListView.SelectedIndex = urlListView.Items.Count - 1;

            urlTextBox.Text = "";
            urlTextBox.Focus();
        }

        /// <summary>
        /// removeButton click event handler
        /// Removes the selected row from the ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            SetDataChanged(true);
            int selectedIndex = urlListView.SelectedIndex;

            urlListView.Items.Remove(urlListView.SelectedItem);

            // if no rows left, add a blank row
            if (urlListView.Items.Count == 0)
            {
                AddRow();
            }
            else if (selectedIndex <= urlListView.Items.Count - 1) // otherwise select next row
            {
                urlListView.SelectedIndex = selectedIndex;
            }
            else // not above cases? Select last row
            {
                urlListView.SelectedIndex = urlListView.Items.Count - 1;
            }
        }

        /// <summary>
        /// urlTextBox TextChanged event handler
        /// Updates the ListView row with current Text 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void urlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshListView(urlTextBox.Text);
        }

        /// <summary>
        /// Refreshses the ListView row with given values
        /// </summary>
        /// <param name="value1">Value for column 1</param>
        private void RefreshListView(string value1)
        {
            ListViewData lvc = (ListViewData)urlListView.SelectedItem; //new ListViewClass(value1, value2);
            if (lvc != null && !_stopRefreshControls)
            {
                SetDataChanged(true);
                lvc.Col1 = value1;
                urlListView.Items.Refresh();
            }
        }

        /// <summary>
        /// urlListView SelectionChnaged event handler.
        /// Updates the textboxes with values in the row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void urlListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewData lvc = (ListViewData)urlListView.SelectedItem;
            if (lvc != null)
            {
                _stopRefreshControls = true;
                urlTextBox.Text = lvc.Col1;
                _stopRefreshControls = false;
            }
        }

        /// <summary>
        /// Window Loaded event handler
        /// Loads data into ListView, and selecta a row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowData();

            if (urlListView.Items.Count == 0)
            {
                AddRow();
            }
            else
            {
                urlListView.SelectedIndex = 0;
            }
            SetDataChanged(false);
            urlTextBox.Focus();
        }

        /// <summary>
        /// Shows(Loads) data into the ListView
        /// </summary>
        private void ShowData()
        {
            MyData md = new MyData();
            urlListView.Items.Clear();

            foreach (var row in md.GetRows())
            {
                urlListView.Items.Add(row);
            }
        }

        /// <summary>
        /// saveButton click event handler.
        /// Saves data from ListView, if it is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton.IsEnabled = false;
            if (_dataChanged)
            {
                MyData md = new MyData();
                md.Save(urlListView.Items);
                SetDataChanged(false);
            }
            saveButton.IsEnabled = true;
        }

        /// <summary>
        /// closeButton click event handler.
        /// Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// okButton click event handler.
        /// Saves the data, and closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            okButton.IsEnabled = false;
            if (_dataChanged)
            {
                MyData md = new MyData();
                md.Save(urlListView.Items);
                SetDataChanged(false);
            }
            Close();
        }

        /// <summary>
        /// Sets the window into a DataChanged status.
        /// </summary>
        /// <param name="value"></param>
        private void SetDataChanged(bool value)
        {
            _dataChanged = value;
            saveButton.IsEnabled = value;
        }

        /// <summary>
        /// Window closing event handler.
        /// Prompts the user to save data, if it is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_dataChanged)
            {
                string message = "Your changes are not saved. Do you want to save it now?";
                MessageBoxResult result = MessageBox.Show(message, Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    MyData md = new MyData();
                    md.Save(urlListView.Items);
                    SetDataChanged(false);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    // do nothing
                }
            }
        }

        /// <summary>
        /// urlTextBox KeyDown event handler.
        /// Restores old value, if Esc key is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                RestoreOldValue(sender);
            }
        }

        /// <summary>
        /// Restores old value, saved in Tag property, to textbox.
        /// </summary>
        /// <param name="sender"></param>
        private void RestoreOldValue(object sender)
        {
            TextBox myText = (TextBox)sender;

            if (myText.Text != myText.Tag.ToString())
            {
                myText.Text = myText.Tag.ToString();
                myText.SelectAll();
            }
        }

        /// <summary>
        /// Saves the current value to the Tag property of textbox.
        /// </summary>
        /// <param name="sender"></param>
        private void StoreCurrentValue(object sender)
        {
            TextBox myText = (TextBox)sender;
            myText.Tag = myText.Text;
        }

        /// <summary>
        /// urlTextBox GotFocus event handler.
        /// Store the current value and select all text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void urlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StoreCurrentValue(sender);
            urlTextBox.SelectAll();
        }

        #endregion

        #region Destination 
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                SelectedPath = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var sPath = folderDialog.SelectedPath;
                tbxFolder.Text = sPath;
                var folder = new DirectoryInfo(sPath);
                if (!folder.Exists) return;
                foreach (FileInfo fileInfo in folder.GetFiles())
                {
                    var sDate = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                    Debug.WriteLine("#Debug: File: " + fileInfo.Name + " Date:" + sDate);
                }
            }
        }
        #endregion

        #region ProcessWork
        private void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Work Done");
            pbStatus.Visibility = Visibility.Hidden;
            pbStatusLabel.Visibility = Visibility.Hidden;
        }

        private void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_mOWorker.CancellationPending)
            {

                e.Cancel = true;
                _mOWorker.ReportProgress(0);
                Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
                {
                    pbStatus.Value = 0;
                    pbStatus.Visibility = Visibility.Hidden;
                    pbStatusLabel.Visibility = Visibility.Hidden;

                });
                return;
            }

            string outputFolder = null;
            List<string> itemList = null;
            string rTargetContent = null;
            RadioButton rTarget;
            RadioButton rViewTarget;
            string[] isMobile = null;
            Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
            {
                outputFolder = tbxFolder.Text;
                pbStatus.Visibility = Visibility.Visible;
                pbStatusLabel.Visibility = Visibility.Visible;

                var buttons = FormatGrid.Children.OfType<RadioButton>().ToList();
                rTarget = buttons.Single(r => r.GroupName == "FormatSelection" && r.IsChecked == true);
                rTargetContent = rTarget.Content.ToString();

                var viewPortButtons = ViewPortGrid.Children.OfType<RadioButton>().ToList();
                rViewTarget = viewPortButtons.Single(r => r.GroupName == "ViewPortSelection" && r.IsChecked == true);
                if (rTargetContent == "PDF" && rViewTarget.Content.ToString() != "MobileView")
                {
                    isMobile = new[] { "--page-size A4 --viewport-size 1280x1024 --disable-smart-shrinking" };
                }
                itemList = (from ListViewData lvc in urlListView.Items where lvc.Col1 != "" select lvc.Col1).ToList();
            });
            var indexer = 1;
            foreach (var item in itemList)
            {
                int percentage;
                var generatorFactory = new GeneratorFactory();
                var generatorType = (GeneratorType)Enum.Parse(typeof(GeneratorType), rTargetContent);
                var generator = generatorFactory.GetGenerator(generatorType);
                _tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var token = _tokenSource.Token;
                var QueryTimeOut = 5000;
                var task = Task.Factory.StartNew(() =>
                {
                    generator.GenerateDocument(outputFolder, "Tx", new[] { item }, isMobile);
                    percentage = indexer * 100 / itemList.Count;
                    _mOWorker.ReportProgress(percentage);
                    indexer += 1;

                }, token);
                if (!task.Wait(QueryTimeOut, token))
                {
                    MessageBox.Show($"Url is taking more than 5 seconds to Load -- Please Check the url: {item}");
                    _tokenSource.Cancel();
                }
                if (task.IsCanceled)
                {
                    MessageBox.Show($"Url not Valid: {item}");
                }
            }
            _mOWorker.ReportProgress(100);
        }
        #endregion

        #region Scheduled Task
        private void CheckScheduleTask()
        {
            if (RunEveryDayCheckBox.IsChecked != true) return;
            if (TimePicker.Value == null) return;
            var trigger = new DailyTrigger(TimePicker.Value.Value.Hour, TimePicker.Value.Value.Minute);
            trigger.OnTimeTriggered += () =>
            {
                MessageBox.Show("Task Scheduled to Run EveryDay at : " + TimePicker.Value.Value.Hour + ":" + TimePicker.Value.Value.Minute);
                pbStatus.Value = 10;
                try
                {
                    _mOWorker.RunWorkerAsync();
                }
                catch (Exception)
                {
                    MessageBox.Show("Problem with Url Data");
                }
            };
        }
        #endregion

        #region ActionButton

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            pbStatus.Value = 10;
            try
            {
                _mOWorker.RunWorkerAsync();
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with Url Data");
            }

            CheckScheduleTask();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource?.Cancel();

            if (_mOWorker.IsBusy)
            {
                _mOWorker.CancelAsync();
            }
        }

        #endregion

    }
}
