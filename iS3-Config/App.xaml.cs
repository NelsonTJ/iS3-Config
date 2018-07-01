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

//** This program is designed for configing data files for iS3.Desktop (standalone version).
//** The data files usually reside in the iS3-Desktop-Stand-alone\Output\Data directory,
//** including:
//**          ProjectList.xml
//**          <ProjectName>.xml
//**          <ProjectName>\*.*
//** This program depends on the following library:
//**          .NET Framework 4.5
//**          ArcGIS Runtime SDK for .NET 10.2.5
//**          iS3.Core
//

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string iS3Path = "";
        string dataPath = "";

        string projName = "";
        double projLocX = 0;
        double projLocY = 0;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // open a background window that start the configuration
            Window backgroundWnd = new Window();
            this.MainWindow = backgroundWnd;

            bool ok = StartConfig();
            if (ok)
            {
                // do something
            }
            else
            {

            }

            Shutdown();
        }

        bool StartConfig()
        {
            bool? success;

            // Step 1 - Config path to iS3 and data directory
            //
            ConfPathWindow mainWnd = new ConfPathWindow();
            success = mainWnd.ShowDialog();
            if (success == null || success.Value == false)
            {
                return false;
            }
            iS3Path = mainWnd.ExePath;
            dataPath = mainWnd.DataPath;

            // Step 2 - Config project title and location information
            //
            string projListFile = dataPath + "\\ProjectList.xml";
            ProjInfoWindow infoWnd = new ProjInfoWindow(projListFile);
            success = infoWnd.ShowDialog();
            if (success == null || success.Value == false)
            {
                return false;
            }
            projName = infoWnd.ProjName;
            projLocX = infoWnd.ProjLocX;
            projLocY = infoWnd.ProjLocY;

            return true;
        }
    }
}
