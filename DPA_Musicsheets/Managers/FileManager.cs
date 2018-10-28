﻿using ClassLibrary.Interfaces;
using DomainModel;
using DPA_Musicsheets.Converters;
using DPA_Musicsheets.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DPA_Musicsheet
{
    public class FileManager
    {
        //private Dictionary<string, ISavable> savables;
        //private readonly Dictionary<string, IReader> readers;
        private ConverterGetter converterGetter;

        //private 
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        public string lilypondText;


        public FileManager()
        {

            converterGetter = new ConverterGetter();
            openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
            saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid|Lilypond|*.ly|PDF|*.pdf" };
        }

        internal Symbol LoadFile(string path)
        {
            string extension = Path.GetExtension(openFileDialog.FileName);
            IReader reader = converterGetter.GetReader(extension);
            IConvertToExtention converter = converterGetter.GetConvertToExtention(".ly");

            if (reader != null && converter != null)
            {
                string fileName = openFileDialog.FileName;
                Symbol root = reader.readFile(fileName);
                lilypondText = converter.Convert(root) as string;
                return root;
            }
            return null;
        }

        public string OpenFile()
        {
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not open the selected file because of the folowing error: " + e);
                return null;

            }
        }

        public void SaveFile(Symbol musicData)
        {

            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);

                ISavable saver = converterGetter.GetSaver(extension);

                if (saver == null)
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                    return;
                }
                saver.Save(saveFileDialog.FileName, musicData);
            }

        }
    }
}
