using System;
using System.Collections.Generic;
using Utils;

namespace Management.UI
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        public bool AdditionalContent { get; private set; }
        private List<Action<bool>> onAdditionalContentChange = new();


        public void AddAdditionalContentListener(Action<bool> action) => onAdditionalContentChange.Add(action);
        public void ToggleAdditionalContent()
        {
            AdditionalContent = !AdditionalContent;
            onAdditionalContentChange.ForEach(action => action.Invoke(AdditionalContent));
        }
    }
}