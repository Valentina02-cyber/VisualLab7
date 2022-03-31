using Avalonia.Media;
using Avalonia;
using ControlPeriod.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace ControlPeriod.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase content;
        
        ObservableCollection<Student> Items { get; set; }
        ObservableCollection<float?> AverageGrades { get; set; }
        ObservableCollection<IBrush> AverageGradesBrushes { get; set; }
        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public void AddNewStudent()
        {
            Items.Insert(0, new Student("Student name"));
            CalculateAveragesOfStudents();
        }

        public void RemoveCheckedStudents()
        {
            var neededStudents = this.Items.Where(x => !x.isChecked).ToList();
            Items.Clear();
            foreach (var neededStudent in neededStudents)
            {
                Items.Add(neededStudent);
            }
            CalculateAveragesOfStudents();

        }

        public MainWindowViewModel()
        {
            Items = new ObservableCollection<Student>();
            AverageGrades = new ObservableCollection<float?>() { 0, 0, 0 };
            AverageGradesBrushes = new ObservableCollection<IBrush>() { new SolidColorBrush(Brushes.White.Color), new SolidColorBrush(Brushes.White.Color), new SolidColorBrush(Brushes.White.Color) };
            Items.CollectionChanged += My1_CollectionChanged;
            Content = new WindowViewModel();
            
        }

        public void WriteToBinaryFile(string filePath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Student>));

            using (StreamWriter wr = new StreamWriter(filePath))
            {
                xs.Serialize(wr, this.Items);
            }
        }

        public void ReadFromBinaryFile(string filePath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Student>));
            using (StreamReader sr = new StreamReader(filePath))
            {
                Items.Clear();
                Items = (ObservableCollection<Student>)xs.Deserialize(sr);
                foreach (Student s in this.Items)
                {
                    var gradeList = new List<Marks>(3);
                    gradeList.Add(s.ControlMarks[3]);
                    gradeList.Add(s.ControlMarks[4]);
                    gradeList.Add(s.ControlMarks[5]);
                    s.ControlMarks.Clear();
                    foreach (var mark in gradeList)
                    {
                        s.ControlMarks.Add(mark);
                    }
                    s.CalculateAverageMark();
                }
            }
        }
        public void OpenWindowView() => Content = new WindowViewModel();

        public void CalculateAveragesOfStudents()
        {

            for (int i = 0; i < 3; i++)
            {
                AverageGrades[i] = 0;
            }
            foreach (Student s in Items)
            {
                if (Items != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        AverageGrades[i] += s.ControlMarks[i].Mark;
                    }
                }
            }
            if (Items.Count != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    AverageGrades[i] /= Items.Count;
                    if (AverageGrades[i] != null)
                    {
                        if (AverageGrades[i] < 1.5) AverageGradesBrushes[i] = new SolidColorBrush(Brushes.Yellow.Color);
                        if (AverageGrades[i] < 1) AverageGradesBrushes[i] = new SolidColorBrush(Brushes.Red.Color);
                        if (AverageGrades[i] >= 1.5) AverageGradesBrushes[i] = new SolidColorBrush(Brushes.LightGreen.Color);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    AverageGrades[i] = null;
                    AverageGradesBrushes[i] = new SolidColorBrush(Brushes.White.Color);
                }

            }
        }
        void My1_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Student item in e.NewItems)
                    item.PropertyChanged += My1_PropertyChanged;
        }

        void My1_PropertyChanged(object sender, PropertyChangedEventArgs e) => CalculateAveragesOfStudents();
    }
}
