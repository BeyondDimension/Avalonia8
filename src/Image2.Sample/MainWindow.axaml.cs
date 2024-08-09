namespace Image2.Sample;

public partial class MainWindow : Window
{
    readonly TabItem mTabItem2;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();

        mTabItem2 = this.Find<TabItem>("TabItem2")!;
        mTabItem2.Header = $"{mTabItem2.Header} {mTabItem2.FontFamily}";
    }
}