﻿using System;
using System.Collections.Generic;
using System.Windows;
using Melville.MVVM.Wpf;
using Melville.MVVM.Wpf.RootWindows;
using Moq;
using NodaTime;
using Planner.Models.Blobs;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Xunit;
using NoteEditorViewModel = Planner.Wpf.Notes.NoteEditorViewModel;

namespace Planner.Wpf.Test.Notes
{
    public class NoteEditorViewModelTest
    {
        private readonly Mock<INotesServer> noteServer = new Mock<INotesServer>();
        private readonly Note note;
        private readonly IList<Note> notes = new List<Note>();
        private readonly Mock<INavigationWindow> navWin = new Mock<INavigationWindow>();
        private readonly Mock<ILocalRepository<Blob>> blobs = new Mock<ILocalRepository<Blob>>();
        private readonly ItemList<Blob> blobList = new ItemList<Blob>();
        private readonly NoteEditorViewModel sut;

        private readonly ZonedDateTime creationTime = new LocalDateTime(1975, 07, 28, 0, 0, 0)
            .InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault());
                

        public NoteEditorViewModelTest()
        {
            blobs.Setup(i => i.ItemsForDate(It.IsAny<LocalDate>())).Returns(blobList);
            noteServer.SetupGet(i => i.BaseUrl).Returns("url://");
            note = new Note()
            {
                Key = Guid.NewGuid(),
                Title = "Title",
                Text = "**Text**",
                TimeCreated = creationTime.ToInstant(),
            };
            notes.Add(note);
            sut = new NoteEditorViewModel(new NoteEditRequestEventArgs(notes, note),
                new NoteUrlGenerator(noteServer.Object), navWin.Object, i=>null, blobs.Object);
        }
        
        [Fact]
        public void NoteUrlIsDifferentEachTimeYouCallIt()
        {
            Assert.Equal($"url://0/{note.Date:yyyy-M-d}/show/{note.Key}", sut.NoteUrl);
            Assert.Equal($"url://1/{note.Date:yyyy-M-d}/show/{note.Key}", sut.NoteUrl);
            Assert.Equal($"url://2/{note.Date:yyyy-M-d}/show/{note.Key}", sut.NoteUrl);
        }

        [Fact]
        public void UpdateUrlOnNoteChange()
        {
            int changes = 0;
            sut.PropertyChanged += (s, e) =>
            {
                Assert.Equal(nameof(NoteEditorViewModel.NoteUrl), e.PropertyName);
                changes++;
            };

            Assert.Equal(0, changes);
            note.Title = "Foo";
            Assert.Equal(1, changes);
            note.Text = "Bar";
            Assert.Equal(2, changes);
        }

        [Fact]
        public void OKTest()
        {
            sut.NavigateToPlannerPage();
            navWin.Verify(i=>i.NavigateTo(null), Times.Once);
            navWin.VerifyNoOtherCalls();
        }

        [Fact]
        public void CancelRevertsTitle()
        {
            note.Title = "Foo";
            sut.CancelEdit();
            navWin.Verify(i=>i.NavigateTo(null), Times.Once);
            navWin.VerifyNoOtherCalls();
            Assert.Equal("Title", note.Title);
            
        }        
        
        [Fact]
        public void CancelRevertsText()
        {
            note.Text = "Foo";
            sut.CancelEdit();
            navWin.Verify(i=>i.NavigateTo(null), Times.Once);
            navWin.VerifyNoOtherCalls();
            Assert.Equal("**Text**", note.Text);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteNote(bool confirmed)
        {
            var msgbox = new Mock<IMessageBoxWrapper>();
            msgbox.Setup(i => i.Show("Do you want to delete: Title", "Planner",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No))
                .Returns(confirmed?MessageBoxResult.Yes:MessageBoxResult.No);
            sut.DeleteNote(msgbox.Object);

            if (confirmed)
            {
                navWin.Verify(i=>i.NavigateTo(null), Times.Once);
            }

            Assert.Equal(!confirmed, notes.Contains(note));
            navWin.VerifyNoOtherCalls();            
        }

        [Fact]
        public void BlobDisplayVisibility()
        {
            Assert.Equal(Visibility.Collapsed, sut.BlobDisplayVisibility);
            sut.Blobs.Add(new Blob());
            Assert.Equal(Visibility.Visible, sut.BlobDisplayVisibility);
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DeleteBlob(bool confirmed)
        {
            var msgbox = new Mock<IMessageBoxWrapper>();
            msgbox.Setup(i => i.Show("Do you want to delete: Title", "Planner",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No))
                .Returns(confirmed?MessageBoxResult.Yes:MessageBoxResult.No);

            var blob = new Blob() {Name = "Title"};
            blobList.Add(blob);
            sut.DeleteBlob(blob, msgbox.Object);
            Assert.Equal(confirmed?0:1, blobList.Count);
        }


    }
}