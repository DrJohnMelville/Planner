﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using Melville.INPC;
using Melville.MVVM.Wpf;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using Microsoft.Web.WebView2.Core;
using NodaTime;
using Planner.Models.Blobs;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Time;
using DailyPlannerPageViewModel = Planner.Wpf.PlannerPages.DailyPlannerPageViewModel;

namespace Planner.Wpf.Notes
{
    [AutoNotify]
    public partial class NoteEditorViewModel: IExternalNotifyPropertyChanged
    {
        public Note Note { get; }
        private readonly IList<Note> notesForDay; 
        private INoteUrlGenerator urlGen;
        private INavigationWindow navigator;
        private Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory;
        private readonly Action cancelOperation;
        public IList<Blob> Blobs { get; }

        public NoteEditorViewModel(
            NoteEditRequestEventArgs request, 
            INoteUrlGenerator urlGen, 
            INavigationWindow navigator, 
            Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory,
            ILocalRepository<Blob> blobStore)
        {
            this.urlGen = urlGen;
            this.navigator = navigator;
            this.plannerPageFactory = plannerPageFactory;
            Note = request.Note;
            notesForDay = request.DailyList;
            cancelOperation = CreateCancelOperation(Note.Title, Note.Text);
            Note.PropertyChanged += UpdateWhenNoteChanges;
            Blobs = blobStore.ItemsForDate(Note.Date);
            NotifyWhenBlobsChange();
        }

        private void NotifyWhenBlobsChange()
        {
            if (Blobs is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += (s, e) => ((IExternalNotifyPropertyChanged) this)
                    .OnPropertyChanged(nameof(BlobDisplayVisibility));
            }
        }

        public void UpdateWhenNoteChanges(object? sender, EventArgs e)=>
            ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NoteUrl));
        
        private Action CreateCancelOperation(string title, string text) => () =>
        {
            Note.Title = title;
            Note.Text = text;
        };

        public string NoteUrl => urlGen.ShowNoteUrl(Note);


        public void NavigateToPlannerPage() => LeavePage(false);
        public void CancelEdit() => LeavePage(true);
        private void LeavePage(bool shouldRevertNote)
        {
            UnhookNoteUpdates();
            if (shouldRevertNote) cancelOperation();
            navigator.NavigateTo(plannerPageFactory(Note.Date));
        }
        private void UnhookNoteUpdates() => Note.PropertyChanged -= UpdateWhenNoteChanges;

        public void DeleteNote([FromServices]IMessageBoxWrapper msgbox)
        {
            if (!UserConfirmsIntentToDelete(msgbox, Note.Title)) return;
            notesForDay.Remove(Note);
            LeavePage(false);
        }

        private bool UserConfirmsIntentToDelete(IMessageBoxWrapper msgbox, string itemToDelete) =>
            msgbox.Show($"Do you want to delete: {itemToDelete}", "Planner",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes;


        public Visibility BlobDisplayVisibility =>
            Blobs.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public void DeleteBlob(Blob blob, [FromServices]IMessageBoxWrapper msgboxObject)
        {
            if (!UserConfirmsIntentToDelete(msgboxObject, blob.Name)) return;
            Blobs.Remove(blob);
        }

        public void OnNavigationStarting(CoreWebView2NavigationStartingEventArgs e) => 
            e.Cancel = !Regex.IsMatch(e.Uri,@"/\d+/\d{4}-\d{1,2}-\d{1,2}/show/");
        
    }

    public class TimeDisplayConverter : IValueConverter
    {
        public IUsersClock? UsersClock { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is Instant instant && UsersClock != null
                ? string.Format(parameter as string??"{0:M/dd/yyyy h:mm tt}",
                   UsersClock.InstantToLocalDateTime(instant))
                : "";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            "";
    }
}