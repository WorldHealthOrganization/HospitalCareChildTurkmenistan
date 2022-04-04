using System;

namespace who_pocket_book.Helpers
{
    public interface IAlertSheet
    {
        void ShowAlert(string title, Action<string> handler, params string[] buttons);
    }
}