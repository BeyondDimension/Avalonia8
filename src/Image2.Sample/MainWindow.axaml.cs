namespace Image2.Sample;

public partial class MainWindow : Window
{
    readonly TabItem mTabItem1;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();

        mTabItem1 = this.Find<TabItem>("TabItem1")!;
        mTabItem1.Header = $"{mTabItem1.Header} {mTabItem1.FontFamily}";
    }
}