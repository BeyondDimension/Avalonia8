namespace Image2.Sample;

public partial class MainWindowViewModel : ReactiveObject
{
    static string[] GetUrlImageSources()
    {
        return [
            "https://6tse5sb49lk6hk0n5edb56hibm4pc0iqom0ocsi2orftnim6hd5vuass.qc.dolfincdnx.net:5147/xdispatch2a304e1874a31533/media.st.dl.eccdnx.com/steamcommunity/public/images/items/1629910/045c57ebb6946fdf7e57a53d5768117dd8543862.gif?bsreqid=f301830fb4dd3faaaa6a682f1482045a&bsxdisp=se",
            "https://image.mossimo.net:5996/images/ys_900x350_0620.jpg",
            "https://bfv6r0mkg5e6b1fhsidqrg5s9oc54o1sd836e0jo17bimn7a684vveag.saxyit.com:5125/steamcommunity/public/images/items/2022180/e081bb44f8c79e9b5ae44d1be71aae559d7fcf88.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMDYsImJzcmVxaWQiOiJmNDdkZTM0ZGIyNTViY2FhNmI5ZmNlNTNiNTRmOTQwZCIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMjAyMjE4MFwvZTA4MWJiNDRmOGM3OWU5YjVhZTQ0ZDFiZTcxYWFlNTU5ZDdmY2Y4OC5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6IjAxYWU1YWU1MmUzZTdiNTQ1NWZhY2Q2ZTdmOTY1NTE0IiwiZm9nMzAyIjoib24ifQ==",
            "https://v52l6aq8roki7o9f7o8na23h34t0ves2e42848c2o7o1t2viot7fupfs.saxyit.com:7843/steamcommunity/public/images/items/1716740/5c9d1755fe58b54c3efa4ae5a37269420b685e91.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMDcsImJzcmVxaWQiOiI4MjA4Zjg0MWRjOGEyZWZjMmE4NTc5ODExODdkNzE3NCIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMTcxNjc0MFwvNWM5ZDE3NTVmZTU4YjU0YzNlZmE0YWU1YTM3MjY5NDIwYjY4NWU5MS5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6ImRmNDIxYTFhNjI4MjIwNjg0MmY0Y2IwMGQ5MTE5ZTU4IiwiZm9nMzAyIjoib24ifQ==",
            "https://bccrltae6c2lrk6q1nheeiq9i78bnfar008le44c6svjnbbs65vvvoea.saxyit.com:7843/steamcommunity/public/images/items/1390700/28a30c70a999c0673453a417a9e5df1ab76bc72d.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMDgsImJzcmVxaWQiOiIxNWZlZTUxNTEzZmM3ODFkODQ2MmFkMmY5NmVmNTJmNSIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMTM5MDcwMFwvMjhhMzBjNzBhOTk5YzA2NzM0NTNhNDE3YTllNWRmMWFiNzZiYzcyZC5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6Ijc5OGMwZDE5MzZmZjhmNzBlMDkxMmM5ZmI2ZWFhOGM4IiwiZm9nMzAyIjoib24ifQ==",
            "https://nk5e6o5jqv4nnmmt506bq6e62m36n059q8q36vi2ismeek8sc1tvvfg7.saxyit.com:7843/steamcommunity/public/images/items/1629910/045c57ebb6946fdf7e57a53d5768117dd8543862.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMDksImJzcmVxaWQiOiJlNTNkYzBhNTc0OWE2OTU0MWZhZDJhOTRlNzllNTFhYiIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMTYyOTkxMFwvMDQ1YzU3ZWJiNjk0NmZkZjdlNTdhNTNkNTc2ODExN2RkODU0Mzg2Mi5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6ImJkOTFjNDNkNjA5OWQ0OWQzNjIzOGM2ODU0MDZhMGFlIiwiZm9nMzAyIjoib24ifQ==",
            "https://sjsqbont38dppkqr5ohmaeip2g0pu799j1qfp93f75dkb0jc4ngvun59.saxyit.com:5124/steamcommunity/public/images/items/774171/ea16deb58fb1582c5ec3acfe5344fdc7b00077de.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMTAsImJzcmVxaWQiOiI1YTViNmFlNDc5YWM4MTJmMWM2YTA3YzljNmVlM2FhMSIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvNzc0MTcxXC9lYTE2ZGViNThmYjE1ODJjNWVjM2FjZmU1MzQ0ZmRjN2IwMDA3N2RlLmdpZiIsImhvc3QzMDIiOiJmb2czMDItc3QuYnM1OGkuYmFpc2hhbmNkbnguY29tIiwia2V5IjoiYjFhOGM1NmY1MjkxYTY1NzU5ZjRiOTNlMjdhNDg4OWUiLCJmb2czMDIiOiJvbiJ9",
            "https://58kvau1ume69avjuut7qr6fkmdnrtt3oqfe7q5471ta53aoot9ivu24p.saxyit.com:7843/steamcommunity/public/images/items/2055500/9fa2a22c70382fcc6658728d23759d1fa36bd61f.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMTEsImJzcmVxaWQiOiJhMWZiOTA3OGRlZjg0ZjJmZjFkYjVlNGFmYjI1N2YyNiIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMjA1NTUwMFwvOWZhMmEyMmM3MDM4MmZjYzY2NTg3MjhkMjM3NTlkMWZhMzZiZDYxZi5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6IjQzNGJiMjZhOWY4MzYwMjQwNzdjMDE4NDI2ZWJlYWQ5IiwiZm9nMzAyIjoib24ifQ==",
            "https://081qvf4f2gacv3trjsl8bcnrdnuaf4fuflc3go86614tj8e8g65fun85.saxyit.com:7843/steamcommunity/public/images/items/2439760/157f2c293251ed449e9479faef78569d14895a43.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMTMsImJzcmVxaWQiOiJhZGUwZGViYmU5MzEzNTBjYjUwMTQ1MzVhNzRjMDdkOCIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMjQzOTc2MFwvMTU3ZjJjMjkzMjUxZWQ0NDllOTQ3OWZhZWY3ODU2OWQxNDg5NWE0My5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6Ijc0NjkwNGQwYmM0MmU2NzNlZTE0ZjNjNjI5ZTUzOWFkIiwiZm9nMzAyIjoib24ifQ==",
            "https://sjsqbont38dppkqr5ohmaeip2g0pu799j1qfp93f75dkb0jc4ngvun59.saxyit.com:5124/steamcommunity/public/images/items/1070330/97227479c36b82d531c866562be67193c691a6d5.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMTQsImJzcmVxaWQiOiJkYzIxMzBlYmEzN2Y5OWY0M2FjOTkwMDQyMGI4ZTRlOSIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMTA3MDMzMFwvOTcyMjc0NzljMzZiODJkNTMxYzg2NjU2MmJlNjcxOTNjNjkxYTZkNS5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6ImNlN2EzNjFlYTk4MDE2OGVlN2Y1Mzk3ODU2ZTA5NmY5IiwiZm9nMzAyIjoib24ifQ==",
            "https://0a0qlbdque417t4pkd6s0qoluf4ie97d8v9nso5333oplenvrkevvm8e.saxyit.com:7843/steamcommunity/public/images/items/1583500/32eaef2a7cacfa8c8a45954ef17b7ccdef570e17.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMTYsImJzcmVxaWQiOiI1OGVlOWNkMGRiY2MyYmVlYTNlNGUyOWIxMTkyNDYwNyIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMTU4MzUwMFwvMzJlYWVmMmE3Y2FjZmE4YzhhNDU5NTRlZjE3YjdjY2RlZjU3MGUxNy5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6IjgzMjQxMDI5NWU5ZjgxOTE1M2Y4MGVjMTllZjBmYWVjIiwiZm9nMzAyIjoib24ifQ==",
            "https://4ak7vqs9q4e5vpv82gkbtec93quu8h3ni24curh1ah7ll62lmh0fvsij.saxyit.com:7843/steamcommunity/public/images/items/2459330/11bbadea5154c316c883df0f3f1944395b3715b8.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMTcsImJzcmVxaWQiOiIwMTY3YjU4MmJlYjBjNGY5ODM4ZDk1NGE5NDc3NDlmMiIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMjQ1OTMzMFwvMTFiYmFkZWE1MTU0YzMxNmM4ODNkZjBmM2YxOTQ0Mzk1YjM3MTViOC5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6Ijg4YTc4OWRkMzEwOWQxMThiY2I3NTI3OGQ2ZDc3MTQ3IiwiZm9nMzAyIjoib24ifQ==",
            "https://123c4htoge6hdlo3p4u1o966jq118btrcq73tea5tpune6crqouvuslb.saxyit.com:7843/steamcommunity/public/images/items/1629910/426703029579f91b5be691806e049fe4d6f59ec3.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMTksImJzcmVxaWQiOiJiNTg0MTUyOWZkMTNmZTEwNDhjNGY5YTk0ZDcwZmFkOCIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMTYyOTkxMFwvNDI2NzAzMDI5NTc5ZjkxYjViZTY5MTgwNmUwNDlmZTRkNmY1OWVjMy5naWYiLCJob3N0MzAyIjoiZm9nMzAyLXN0LmJzNThpLmJhaXNoYW5jZG54LmNvbSIsImtleSI6ImQyMGFlOWRmNzI3OTE3MTk0NjY5MTJiZjJiNTE2MGMzIiwiZm9nMzAyIjoib24ifQ==",
            "https://0ck5bgcoj7aj4pt9p9mfh35djkoifbtfopu30n74o7v5l7sgrmdvudgk.saxyit.com:7843/steamcommunity/public/images/items/216150/5933160d2b734175fd7e7adbeb894fc1b4a02f08.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMjAsImJzcmVxaWQiOiIwMWExZDI1MzllNzEyYWIyMzgxMjZjOGM0ZjVjMmJhNiIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvMjE2MTUwXC81OTMzMTYwZDJiNzM0MTc1ZmQ3ZTdhZGJlYjg5NGZjMWI0YTAyZjA4LmdpZiIsImhvc3QzMDIiOiJmb2czMDItc3QuYnM1OGkuYmFpc2hhbmNkbnguY29tIiwia2V5IjoiYzg2ZDY5MDhmYzIzNmQ1YjljZmI1ZTY5Nzg5NjdkNjkiLCJmb2czMDIiOiJvbiJ9",
            "https://qaujqg8ogvdiih82aksfpr73htqpl2ab17mhrumlokr8p9qlo81vvha1.saxyit.com:7843/steamcommunity/public/images/items/594650/36d25858eb7d74231188b8f7b53201ff79463b8c.gif?BSLuBan=eyJob3N0IjoibWVkaWEuc3QuZGwuZWNjZG54LmNvbSIsImRuc19ob3N0IjoienRnZGwudi50cnBjZG4ubmV0IiwidHMiOjE3MDAxMzMzMjMsImJzcmVxaWQiOiIyYjljMjdlZTA3NDhmOGQyNjRmMTBlZTg5ODc4OWIyMCIsImNhY2hlX2tleSI6Ilwvc3RlYW1jb21tdW5pdHlcL3B1YmxpY1wvaW1hZ2VzXC9pdGVtc1wvNTk0NjUwXC8zNmQyNTg1OGViN2Q3NDIzMTE4OGI4ZjdiNTMyMDFmZjc5NDYzYjhjLmdpZiIsImhvc3QzMDIiOiJmb2czMDItc3QuYnM1OGkuYmFpc2hhbmNkbnguY29tIiwia2V5IjoiN2U5N2VjNTU3NmE4ODYwODMxMDRiYTIzZGRiM2I3MTciLCJmb2czMDIiOiJvbiJ9",
        ];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        _stretches = new List<Stretch>
        {
            Stretch.None,
            Stretch.Fill,
            Stretch.Uniform,
            Stretch.UniformToFill,
        };

        var list = AssetLoader.GetAssets(new Uri("avares://Image2.Sample/Images/"), null)
              .Select(x =>
              {
                  using var stream = AssetLoader.Open(x);
                  if (FileFormat.IsImage(stream, out var format))
                  {
                      return new ImageSource
                      {
                          Format = format,
                          Path = x.AbsoluteUri,
                      };
                  }
                  return new ImageSource
                  {
                      Path = x.AbsoluteUri,
                  };
              }).ToList();
        list.AddRange(GetUrlImageSources().Select(x => new ImageSource(x)));
        _availableGifs = list;
    }

    private IReadOnlyList<ImageSource> _availableGifs;

    public IReadOnlyList<ImageSource> AvailableGifs
    {
        get => _availableGifs;
        set => this.RaiseAndSetIfChanged(ref _availableGifs, value);
    }

    private ImageSource? _selectedGif;

    public ImageSource? SelectedGif
    {
        get => _selectedGif;
        set => this.RaiseAndSetIfChanged(ref _selectedGif, value);
    }

    private IReadOnlyList<Stretch> _stretches;

    public IReadOnlyList<Stretch> Stretches
    {
        get => _stretches;
        set => this.RaiseAndSetIfChanged(ref _stretches, value);
    }

    private Stretch _stretch = Stretch.None;

    public Stretch Stretch
    {
        get => _stretch;
        set => this.RaiseAndSetIfChanged(ref _stretch, value);
    }

    public ICommand HangUiThreadCommand { get; } =
        ReactiveCommand.Create(() => Dispatcher.UIThread.InvokeAsync(() => Thread.Sleep(5000)));
}

public record class ImageSource
{
    public ImageSource() { }

    public ImageSource(string path) => Path = path;

    public string Path { get; init; } = null!;

    public ImageFormat Format { get; init; }

    public override string ToString()
    {
        return $"[{Format}] {Path}";
    }
}