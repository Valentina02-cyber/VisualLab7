using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ControlPeriod.Models
{
    public class Student : INotifyPropertyChanged
    {
        public string Name { set; get; }

        ObservableCollection<Marks> controlMarks;
        public ObservableCollection<Marks> ControlMarks
        {
            get => controlMarks;
            set
            {
                this.controlMarks = value;
                RaisePropertyChangedEvent("ControlMarks");
            }
        }

        float? average;
        [XmlIgnore]
        Avalonia.Media.SolidColorBrush averageBrush;
        [XmlIgnore]
        public Avalonia.Media.SolidColorBrush AverageBrush
        {
            get => averageBrush;
            private set
            {
                this.averageBrush = value;
                RaisePropertyChangedEvent("AverageBrush");
            }
        }
        [XmlIgnore]
        public bool isChecked { get; set; }
        [XmlIgnore]
        public float? Average
        {
            get => average;
            private set
            {
                if (value is not null)
                {
                    if (value < 1.5)
                    {
                        AverageBrush = new SolidColorBrush(Brushes.Yellow.Color);
                        average = value;
                    }
                    if (value < 1)
                    {
                        AverageBrush = new SolidColorBrush(Brushes.Red.Color);
                        average = value;
                    }
                    if (value >= 1.5)
                    {
                        AverageBrush = new SolidColorBrush(Brushes.LightGreen.Color);
                        average = value;
                    }
                }
                else
                {
                    average = null;
                    AverageBrush = new SolidColorBrush(Brushes.White.Color);
                }
                RaisePropertyChangedEvent("Average");
            }
        }

        public void CalculateAverageMark()
        {
            if (ControlMarks.Any(mark => mark.Mark is null))
            {
                Average = null;
            }
            else
            {
                float sum = 0;
                foreach (var mark in ControlMarks) sum += (float)mark.Mark;
                Average = sum / 3;
            }
        }
        public Student(string name)
        {
            Name = name;
            ControlMarks = new ObservableCollection<Marks>();
            ControlMarks.CollectionChanged += My_CollectionChanged;
            ControlMarks.Clear();
            ControlMarks.Add(new Marks(0));
            ControlMarks.Add(new Marks(0));
            ControlMarks.Add(new Marks(0));
            isChecked = false;
            CalculateAverageMark();
        }

        public Student()
        {
            Name = "NULL";
            ControlMarks = new ObservableCollection<Marks>();
            ControlMarks.CollectionChanged += My_CollectionChanged;
            ControlMarks.Clear();
            ControlMarks.Add(new Marks(0));
            ControlMarks.Add(new Marks(0));
            ControlMarks.Add(new Marks(0));
            isChecked = false;
            CalculateAverageMark();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChangedEvent(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        void My_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Marks item in e.NewItems)
                    item.PropertyChanged += My_PropertyChanged;

            if (e.OldItems != null)
                foreach (Marks item in e.OldItems)
                    item.PropertyChanged -= My_PropertyChanged;
        }

        void My_PropertyChanged(object sender, PropertyChangedEventArgs e) => CalculateAverageMark();
     

    }
}
