using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DiscUtils.Iso9660;
using IsoCreator.Domain;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace IsoCreator
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static MainViewModel CreateEmptyViewModel() => new MainViewModel();

        public static MainViewModel CreateViewModel(string path)
        {
            var viewModel = new MainViewModel();
            viewModel.SourcePath = path;
            viewModel.SetDefaultDestinantion();
            return viewModel;
        }

        private MainViewModel()
        {
            SelectSource = new DelegateCommand(SelectSourceExecute);
            SelectDestination = new DelegateCommand(SelectDestinationExecute);
            RunCommand = new DelegateCommand(RunExecute);
            IsNotRunning = true;
        }

        public ICommand SelectSource { get; set; }
        public ICommand SelectDestination { get; set; }
        public ICommand RunCommand { get; set; }

        public async void RunExecute()
        {
            try
            {
                if(!ValidatePaths()) return;
                IFilesLoader filesLoader = new FilesLoader();
                CDBuilder cdBuilder = new CDBuilder();
                foreach (var (nameInIso, fullName) in filesLoader.GetFiles(SourcePath))
                {
                    cdBuilder.AddFile(nameInIso, fullName);
                }

                IsNotRunning = false;
                await cdBuilder.BuildAsync(DestinationPath,
                    (current, max) => ProgressValue = current / (double)max * 100);
                ProgressValue = 100;
                IsNotRunning = true;
                MessageBox.Show("Operation complete");
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Problem has occurred during processing. Details below:{Environment.NewLine}{exception.Message}");
                IsNotRunning = true;
                ProgressValue = 0;
            }
        }

        private bool ValidatePaths()
        {
            if (!Directory.Exists(SourcePath))
            {
                MessageBox.Show("Source directory doesn't exist.");
                return false;
            }
            if (!Path.IsPathFullyQualified(DestinationPath))
            {
                MessageBox.Show("Destination path is not correct.");
                return false;
            }
            return true;
        }

        private void SelectDestinationExecute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "output.iso";
            saveFileDialog.ShowDialog();
            DestinationPath = saveFileDialog.FileName;
        }

        private void SelectSourceExecute()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SourcePath = dialog.FileName;
            }
            if(string.IsNullOrWhiteSpace(DestinationPath))
                SetDefaultDestinantion();
        }

        private void SetDefaultDestinantion()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(SourcePath);
            DestinationPath = $"{directoryInfo.Parent?.FullName}\\{directoryInfo.Name}.iso";
        }


        private string _sourcePath;
        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                if (_sourcePath != value)
                {
                    _sourcePath = value;
                    OnPropertyChanged(nameof(SourcePath));
                }
            }
        }

        private string _destinationPath;
        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                if (_destinationPath != value)
                {
                    _destinationPath = value;
                    OnPropertyChanged(nameof(DestinationPath));
                }
            }
        }

        private double _progressValue;
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged(nameof(ProgressValue));
                }
            }
        }

        private bool _isNotRunning;

        public bool IsNotRunning
        {
            get => _isNotRunning;
            set
            {
                if (_isNotRunning != value)
                {
                    _isNotRunning = value;
                    OnPropertyChanged(nameof(IsNotRunning));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
