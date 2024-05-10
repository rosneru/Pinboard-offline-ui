using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Logic.UI.ViewModels.Controls
{
  public class MessageBoxViewModel : ObservableObject
  {

    public ICommand CmdLeftButton { get; }
    public ICommand CmdMiddleButton { get; }
    public ICommand CmdRightButton { get; }

    public enum Style
    {
      Normal,
      Good,
      Warning,
      Error
    }

    public string BackgroundColor
    {
      get { return backgroundColor; }
      set 
      {
        backgroundColor = value;
        OnPropertyChanged(nameof(BackgroundColor));
      }
    }

    public string TextColor
    {
      get { return textColor; }
      set
      {
        textColor = value;
        OnPropertyChanged(nameof(TextColor));
      }
    }

    public string MessageText
    {
      get { return messageText; }
      set
      {
        if (value != messageText)
        {
          messageText = value;
          OnPropertyChanged(nameof(MessageText));
        }
      }
    }

    public string MessageQuestion
    {
      get { return messageQuestion; }
      set
      {
        if (value != messageQuestion)
        {
          messageQuestion = value;
          OnPropertyChanged(nameof(MessageQuestion));
        }
      }
    }


    public string LeftButtonText
    {
      get { return leftButtonText; }
      set
      {
        if (value != leftButtonText)
        {
          leftButtonText = value;
          OnPropertyChanged(nameof(LeftButtonText));
          OnPropertyChanged(nameof(IsLeftButtonVisible));
        }
      }
    }

    public string MiddleButtonText
    {
      get { return middleButtonText; }
      set
      {
        if (value != middleButtonText)
        {
          middleButtonText = value;
          OnPropertyChanged(nameof(MiddleButtonText));
          OnPropertyChanged(nameof(IsMiddleButtonVisible));
        }
      }
    }

    public string RightButtonText
    {
      get { return rightButtonText; }
      set
      {
        if (value != rightButtonText)
        {
          rightButtonText = value;
          OnPropertyChanged(nameof(RightButtonText));
          OnPropertyChanged(nameof(IsRightButtonVisible));
        }
      }
    }


    public bool IsLeftButtonVisible
    {
      get { return !string.IsNullOrEmpty(LeftButtonText); }
    }

    public bool IsMiddleButtonVisible
    {
      get { return !string.IsNullOrEmpty(MiddleButtonText); }
    }

    public bool IsRightButtonVisible
    {
      get { return !string.IsNullOrEmpty(RightButtonText); }
    }


    public bool IsOpen
    {
      get { return isOpen; }
      set
      {
        if (value != isOpen)
        {
          isOpen = value;
          OnPropertyChanged(nameof(IsOpen));
        }
      }
    }


    public MessageBoxViewModel(ICommand cmdMenuLock,
                               ICommand cmdMenuUnlock)
    {
      this.cmdMenuLock = cmdMenuLock;
      this.cmdMenuUnlock = cmdMenuUnlock;

      this.backgroundColors = new Dictionary<Style, string>();
      this.backgroundColors[Style.Normal] = "#CCD7E0";
      this.backgroundColors[Style.Good] = "#91FF91";
      this.backgroundColors[Style.Warning] = "#FEFF91";
      this.backgroundColors[Style.Error] = "#F3AFAF";

      this.textColors = new Dictionary<Style, string>();
      this.textColors[Style.Normal] = "#005BAC";
      this.textColors[Style.Good] = "#005BAC";
      this.textColors[Style.Warning] = "#005BAC";
      this.textColors[Style.Error] = "#005BAC";

      this.backgroundColor = this.backgroundColors[Style.Normal];
      this.textColor = this.textColors[Style.Normal];

      CmdLeftButton = new RelayCommand(() =>
      {
        if(!IsOpen)
        {
          return;
        }

        // Close the dialog
        cmdMenuUnlock.Execute(null);
        IsOpen = false;

        // Then execute the provided command
        cmd1?.Execute(null);
      }, () => true);

      CmdMiddleButton = new RelayCommand(() =>
      {
        if (!IsOpen)
        {
          return;
        }

        // First close the dialog
        cmdMenuUnlock.Execute(null);
        IsOpen = false;

        // Then execute the provided command
        cmd2?.Execute(null);
      }, () => true);

      CmdRightButton = new RelayCommand(() =>
      {
        if (!IsOpen)
        {
          return;
        }

        // First close the dialog
        cmdMenuUnlock.Execute(null);
        IsOpen = false;

        // Then execute the provided command
        cmd3?.Execute(null);
      }, () => true);

    }

    /// <summary>
    /// Displays a message box with two lines of message text and one
    /// button. The button text is set to the given button text. When
    /// clicked the given command is executed.
    /// </summary>
    public void Show(string message1,
                     string message2,
                     ICommand cmd, string buttonText)
    {
      Show(Style.Normal,
           message1,
           message2,
           cmd, buttonText,
           null, "",
           null, "");
    }

    /// <summary>
    /// Displays a message box with two lines of message text and one
    /// button. The button text is set to the given button text. When
    /// clicked the given command is executed.
    /// </summary>
    public void Show(Style style,
                     string message1,
                     string message2,
                     ICommand cmd, string buttonText)
    {
      Show(style,
        message1,
        message2,
        cmd, buttonText,
        null, "",
        null, "");
    }

    /// <summary>
    /// Displays a message box with two lines of message text and two
    /// buttons. For each button both, the displayed text and a command 
    /// to be executed on button click must be provided.
    /// </summary>
    public void Show(string message1,
                     string message2,
                     ICommand leftCmd, string leftButtonText,
                     ICommand rightCmd, string rightButtonText)
    {
      Show(Style.Normal,
           message1,
           message2,
           leftCmd, leftButtonText,
           null, "",
           rightCmd, rightButtonText);
    }

    /// <summary>
    /// Displays a message box with two lines of message text and two
    /// buttons. For each button both, the displayed text and a command 
    /// to be executed on button click must be provided.
    /// </summary>
    public void Show(Style style,
                     string message1,
                     string message2,
                     ICommand leftCmd, string leftButtonText,
                     ICommand rightCmd, string rightButtonText)
    {
      Show(style,
           message1,
           message2,
           leftCmd, leftButtonText,
           null, "",
           rightCmd, rightButtonText);
    }

    /// Displays a message box with two lines of message text and three
    /// buttons. For each button both, the displayed text and a command 
    /// to be executed on button click must be provided.
    public void Show(string message1,
                     string message2,
                     ICommand leftCmd, string leftButtonText,
                     ICommand middleCmd, string middleButtonText,
                     ICommand rightCmd, string rightButtonText)
    {
      Show(Style.Normal,
           message1,
           message2,
           leftCmd, leftButtonText,
           middleCmd, middleButtonText,
           rightCmd, rightButtonText);
    }

    /// Displays a message box with two lines of message text and three
    /// buttons. For each button both, the displayed text and a command 
    /// to be executed on button click must be provided.
    public void Show(Style style,
                     string message1,
                     string message2,
                     ICommand leftCmd, string leftButtonText,
                     ICommand middleCmd, string middleButtonText,
                     ICommand rightCmd, string rightButtonText)
    {
      BackgroundColor = backgroundColors[style];
      TextColor = textColors[style];

      MessageText = message1;
      MessageQuestion = message2;

      this.cmd1 = leftCmd;
      this.cmd2 = middleCmd;
      this.cmd3 = rightCmd;

      LeftButtonText = leftButtonText;
      MiddleButtonText = middleButtonText;
      RightButtonText = rightButtonText;

      cmdMenuLock.Execute(null);
      IsOpen = true;
    }


    private ICommand cmd1;
    private ICommand cmd2;
    private ICommand cmd3;
    
    private ICommand cmdMenuLock;
    private ICommand cmdMenuUnlock;
    private bool isOpen;

    private readonly Dictionary<Style,string> backgroundColors;
    private readonly Dictionary<Style, string> textColors;

    private string backgroundColor;
    private string textColor;

    private string messageText;
    private string messageQuestion;

    private string leftButtonText;
    private string middleButtonText;
    private string rightButtonText;
  }

}
