//************************  Notice  **********************************
//** This file is part of iS3
//**
//** Copyright (c) 2018 Tongji University iS3 Team. All rights reserved.
//**
//** This library is free software; you can redistribute it and/or
//** modify it under the terms of the GNU Lesser General Public
//** License as published by the Free Software Foundation; either
//** version 3 of the License, or (at your option) any later version.
//**
//** This library is distributed in the hope that it will be useful,
//** but WITHOUT ANY WARRANTY; without even the implied warranty of
//** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//** Lesser General Public License for more details.
//**
//** In addition, as a special exception,  that plugins developed for iS3,
//** are allowed to remain closed sourced and can be distributed under any license .
//** These rights are included in the file LGPL_EXCEPTION.txt in this package.
//**
//**************************************************************************
//**
//** The class in this file is used to configure the ProjectList.xml
//**
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
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Windows.Markup;
using System.Xml.Serialization;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ProjInfoWindow.xaml
    /// </summary>
    public partial class ProjInfoWindow : Window
    {
        public string ProjName = "";
        public double ProjLocX = 0;
        public double ProjLocY = 0;

        string _projListFile;
        ProjectList _projList;
        GraphicsLayer _gLayer;
        PictureMarkerSymbol _pinMarkerSymbol = new PictureMarkerSymbol();

        public ProjInfoWindow(string projListFile)
        {
            InitializeComponent();

            _projListFile = projListFile;

            InitializePictureMarkerSymbol();
            MyMapView.Loaded += MyMapView_Loaded;
            MyMapView.MouseDown += MyMapView_MouseDown;

            LoadProjectList();
            if (_projList == null)
                return;
            foreach (ProjectLocation loc in _projList.Locations)
            {
                ProjectListLB.Items.Add(loc);
            }
        }

        private async void InitializePictureMarkerSymbol()
        {
            try
            {
                await _pinMarkerSymbol.SetSourceAsync(
                    new Uri("pack://application:,,,/IS3.Config;component/Images/pin_red.png"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        void LoadProjectList()
        {
            try
            {
                // Load ProjectList.xml using XamlReader
                StreamReader reader = new StreamReader(_projListFile);
                object obj = XamlReader.Load(reader.BaseStream);
                _projList = obj as ProjectList;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
            }
        }

        void WriteProjectList()
        {
            try
            {
                // write xml to memory stream at first
                Stream memStream = new MemoryStream();
                XmlAttributeOverrides overide = CreateOverrides();  // overide some attributes
                XmlSerializer s = new XmlSerializer(typeof(ProjectList), overide);
                s.Serialize(memStream, _projList);  // write to memory

                // replace "Locations" with "ProjectList.Locations" so XamlReader would be happy.
                memStream.Position = 0;
                StreamReader reader = new StreamReader(memStream);
                string xml = reader.ReadToEnd();
                string xaml = xml.Replace("Locations", "ProjectList.Locations");

                // overide ProjectList.xml
                FileStream fs = new FileStream(_projListFile, FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(xaml);
                writer.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
            }
        }

        // Write ProjectList member variables (XMin, XMax, YMin, Ymax, UseGeographicMap)
        // and ProjectLocation member variables (ID, DefinitionFile, X, Y, Description, Default)
        // as attributes into XML, so XamlReader can work happily.
        XmlAttributeOverrides CreateOverrides()
        {
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();

            // add root element namespace
            attrOverrides.Add(typeof(ProjectList), new XmlAttributes()
            {
                XmlRoot = new XmlRootAttribute()
                {
                    ElementName = "ProjectList",
                    Namespace = "clr-namespace:IS3.Core;assembly=IS3.Core"
                }
            });

            // write ProjectList member variables as attributes
            attrOverrides.Add(typeof(ProjectList), "XMax", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("XMax") });
            attrOverrides.Add(typeof(ProjectList), "XMin", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("XMin") });
            attrOverrides.Add(typeof(ProjectList), "YMax", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("YMax") });
            attrOverrides.Add(typeof(ProjectList), "YMin", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("YMin") });
            attrOverrides.Add(typeof(ProjectList), "UseGeographicMap", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("UseGeographicMap") });

            // write ProjectLocation member variables as attributes
            attrOverrides.Add(typeof(ProjectLocation), "ID", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("ID") });
            attrOverrides.Add(typeof(ProjectLocation), "DefinitionFile", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("DefinitionFile") });
            attrOverrides.Add(typeof(ProjectLocation), "X", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("X") });
            attrOverrides.Add(typeof(ProjectLocation), "Y", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("Y") });
            attrOverrides.Add(typeof(ProjectLocation), "Description", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("Description") });
            attrOverrides.Add(typeof(ProjectLocation), "Default", new XmlAttributes()
                { XmlAttribute = new XmlAttributeAttribute("Default") });

            return attrOverrides;
        }

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            _gLayer = Map.Layers["ProjectGraphicsLayer"] as GraphicsLayer;
            if (_projList == null)
                return;
            foreach (ProjectLocation loc in _projList.Locations)
            {
                AddProjectLocationOnMap(loc);
            }
        }

        void AddProjectLocationOnMap(ProjectLocation loc)
        {
            Graphic g = new Graphic()
            {
                Geometry = new MapPoint(loc.X, loc.Y),
                Symbol = _pinMarkerSymbol,
            };
            g.Attributes["ID"] = loc.ID;
            g.Attributes["DefinitionFile"] = loc.DefinitionFile;
            g.Attributes["Description"] = loc.Description;

            _gLayer.Graphics.Add(g);
        }

        private void ProjectListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
            {
                ProjectTitleTB.Visibility = Visibility.Hidden;
                _gLayer.Graphics.Clear();
                return;
            }

            ProjectTitleTB.Visibility = Visibility.Visible;
            ProjectTitleTB.Text = loc.Description;
            ProjectTitleTB.IsReadOnly = true;

            _gLayer.Graphics.Clear();
            AddProjectLocationOnMap(loc);

            PromptTB.Text = "Select operations from the configuration.";
            StepLB.SelectedIndex = -1;
        }

        private void StepLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            int step = StepLB.SelectedIndex;
            if (step == 0)
            {
                PromptTB.Text = "Input project title in the title text box.";
                ProjectTitleTB.IsReadOnly = false;
                ProjectTitleTB.Focus();
                ProjectTitleTB.SelectAll();
                ProjectTitleTB.Visibility = System.Windows.Visibility.Visible;
                _gLayer.Graphics.Clear();
            }
            else if (step == 1)
            {
                PromptTB.Text = "Drop the project location on the map.";
                ProjectTitleTB.Visibility = System.Windows.Visibility.Hidden;
                AddProjectLocationOnMap(loc);
            }
        }

        private void ProjectTitleTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            loc.Description = ProjectTitleTB.Text;
        }

        void MyMapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            int step = StepLB.SelectedIndex;
            if (step != 1)
                return;

            Point screenPoint = e.GetPosition(MyMapView);
            MapPoint coord = MyMapView.ScreenToLocation(screenPoint);
            _gLayer.Graphics.Clear();

            loc.X = coord.X;
            loc.Y = coord.Y;
            AddProjectLocationOnMap(loc);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddProjWindow addProjWnd = new AddProjWindow();
            addProjWnd.Owner = this;
            addProjWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = addProjWnd.ShowDialog();
            if (ok!=null && ok.Value==true)
            {
                ProjectLocation loc = new ProjectLocation();
                loc.ID = addProjWnd.IdTB.Text;
                loc.Description = addProjWnd.DescTB.Text;
                loc.DefinitionFile = loc.ID + ".xml";
                loc.X = 0;
                loc.Y = 0;
                _projList.Locations.Add(loc);

                ProjectListLB.Items.Add(loc);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            _projList.Locations.Remove(loc);
            ProjectListLB.Items.Remove(loc);

            ProjectListLB.SelectedIndex = -1;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            WriteProjectList();
            // finish 
            DialogResult = true;
            Close();
        }

    }
}
