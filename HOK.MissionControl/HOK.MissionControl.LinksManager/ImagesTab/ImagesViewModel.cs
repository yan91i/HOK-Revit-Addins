﻿using System.Collections;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using HOK.Core.ElementWrapers;
using HOK.Feedback;

namespace HOK.MissionControl.LinksManager.ImagesTab
{
    public class ImagesViewModel : ObservableRecipient
    {
        public IList SelectedRows { get; set; }
        public ImagesModel Model { get; set; }
        public ObservableCollection<ImageTypeWrapper> Images { get; set; }
        public RelayCommand Delete { get; }
        public RelayCommand<UserControl> Close { get; }
        public RelayCommand SubmitComment { get; set; }
        public RelayCommand SelectAll { get; set; }
        public RelayCommand SelectNone { get; set; }
        public RelayCommand<bool> Check { get; set; }

        public ImagesViewModel(ImagesModel model)
        {
            Model = model;
            Images = Model.Images;

            Delete = new RelayCommand(OnDelete);
            Close = new RelayCommand<UserControl>(OnClose);
            SubmitComment = new RelayCommand(OnSubmitComment);
            SelectAll = new RelayCommand(OnSelectAll);
            SelectNone = new RelayCommand(OnSelectNone);
            Check = new RelayCommand<bool>(OnCheck);
        }

        private void OnSelectNone()
        {
            foreach (var wrapper in Images)
            {
                wrapper.IsSelected = false;
            }
        }

        private void OnSelectAll()
        {
            foreach (var wrapper in Images)
            {
                wrapper.IsSelected = true;
            }
        }

        private void OnCheck(bool isChecked)
        {
            var selected = SelectedRows.Cast<ImageTypeWrapper>().ToList();
            if (!selected.Any()) return;

            foreach (var wrapper in selected)
            {
                wrapper.IsSelected = isChecked;
            }
        }

        private static void OnSubmitComment()
        {
            var title = "Mission Control - Links Manager v." + Assembly.GetExecutingAssembly().GetName().Version;
            var model = new FeedbackModel();
            var viewModel = new FeedbackViewModel(model, title);
            var view = new FeedbackView
            {
                DataContext = viewModel
            };

            var unused = new WindowInteropHelper(view)
            {
                Owner = Process.GetCurrentProcess().MainWindowHandle
            };

            view.ShowDialog();
        }

        private void OnDelete()
        {
            var selected = (from ImageTypeWrapper s in Images where s.IsSelected select s).ToList();
            var deleted = Model.Delete(selected);

            foreach (var i in deleted)
            {
                Images.Remove(i);
            }
        }

        private static void OnClose(UserControl control)
        {
            var win = Window.GetWindow(control);
            win?.Close();
        }
    }
}
