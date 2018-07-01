﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ocph.DAL
{
    public  class  BaseNotify : INotifyPropertyChanged
    {
        public  bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private  void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private bool isBusy;
        private string _title;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref  isBusy ,value); }
        }

        public string MyTitle
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }


    }
}
