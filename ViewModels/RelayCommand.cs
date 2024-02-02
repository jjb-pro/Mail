using System;
using System.Windows.Input;

namespace Mail.ViewModels;

public class RelayCommand(Action action, Func<bool>? canExecute = null) : ICommand
{
    private readonly Action _action = action ?? throw new ArgumentNullException(nameof(action));
    private readonly Func<bool>? _canExecute = canExecute;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

    public void Execute(object? parameter) => _action();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}