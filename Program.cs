using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Terminal.Gui;

class Lista
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public double Cena { get; set; }
    public int Ilosc { get; set; }

    public override string ToString()
    {
        return $"ID: {Id}; Nazwa: {Nazwa}; Cena: {Cena}; Ilość: {Ilosc}";
    }
}

class Program
{
    static List<Lista> produkty = new List<Lista>();
    static string filePath = "produkty.csv";

    static ListView listView;

    static void Main()
    {
        WczytajZCsv();

        Application.Init();
        var top = Application.Top;

        var win = new Window("Magazyn Produktów")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        // Menu
        var menu = new MenuBar(new MenuBarItem[]
        {
            new MenuBarItem ("Plik", new MenuItem []
            {
                new MenuItem ("Zapisz do CSV", "", () => ZapiszDoCsv()),
                new MenuItem ("Wyjście", "", () => { Application.RequestStop(); })
            }),
            new MenuBarItem ("Produkty", new MenuItem []
            {
                new MenuItem ("Dodaj", "", () => DodajProduktGUI()),
                new MenuItem ("Edytuj", "", () => EdytujProduktGUI()),
                new MenuItem ("Usuń", "", () => UsunProduktGUI())
            })
        });

        listView = new ListView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        OdswiezListe();

        win.Add(listView);
        top.Add(menu, win);

        Application.Run();

        // Po wyjściu z GUI - przechodzi do starego menu konsolowego
        MainKonsolowe();
    }

    static void MainKonsolowe()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Wybierz opcję:");
            Console.WriteLine("1) Dodaj produkt");
            Console.WriteLine("2) Pokaż produkty");
            Console.WriteLine("3) Edytuj produkt");
            Console.WriteLine("4) Usuń produkt");
            Console.WriteLine("5) Eksport do CSV");
            Console.WriteLine("0) Wyjście");
            Console.Write(">> ");

            string opcja = Console.ReadLine();
            switch (opcja)
            {
                case "1":
                    DodajProdukt();
                    break;
                case "2":
                    PokazProdukt();
                    break;
                case "3":
                    EdytujProdukt();
                    break;
                case "4":
                    UsunProdukt();
                    break;
                case "5":
                    ZapiszDoCsv();
                    break;
                case "0":
                    Console.WriteLine("Koniec programu");
                    return;
                default:
                    Console.WriteLine("Nieprawidłowa opcja!");
                    break;
            }
        }
    }

    static void DodajProdukt()
    {
        int noweID = produkty.Count == 0 ? 1 : produkty.Max(p => p.Id) + 1;
        Console.WriteLine("Napisz nazwę produktu:");
        string nazwa = Console.ReadLine();

        double cena = WczytajCene();
        int ilosc = WczytajIlosc();

        produkty.Add(new Lista { Id = noweID, Nazwa = nazwa, Ilosc = ilosc, Cena = cena });
        Console.WriteLine($"Dodano Produkt: ID = {noweID}, Nazwa = {nazwa}, Ilość = {ilosc}, Cena = {cena}");
        OdswiezListe();
    }

    static double WczytajCene()
    {
        double cena = 0;
        while (true)
        {
            try
            {
                Console.WriteLine("Napisz cenę produktu:");
                string cenaInput = Console.ReadLine().Trim();
                cenaInput = cenaInput.Replace('.', ',');
                cena = double.Parse(cenaInput);
                if (cena <= 0)
                    throw new Exception("Cena musi być większa od zera!");
                return cena;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
    }

    static int WczytajIlosc()
    {
        int ilosc = 0;
        while (true)
        {
            try
            {
                Console.WriteLine("Napisz ilość produktów:");
                string iloscInput = Console.ReadLine().Trim();
                if (iloscInput.Contains(".") || iloscInput.Contains(","))
                    throw new Exception("Ilość musi być liczbą całkowitą!");
                ilosc = int.Parse(iloscInput);
                if (ilosc <= 0)
                    throw new Exception("Ilość musi być większa od zera!");
                return ilosc;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
    }

    static void PokazProdukt()
    {
        if (produkty.Count == 0)
        {
            Console.WriteLine("Brak produktów");
        }
        else
        {
            Console.WriteLine("\nLista Produktów:");
            Console.WriteLine($"Liczba produktów: {produkty.Count}");
            foreach (var p in produkty)
            {
                Console.WriteLine(p);
            }
        }
    }

    static void EdytujProdukt()
    {
        Console.WriteLine("Podaj ID produktu:");
        if (!int.TryParse(Console.ReadLine(), out int Id))
        {
            Console.WriteLine("Nie ma takiego ID!");
            return;
        }

        var produkt = produkty.Find(p => p.Id == Id);
        if (produkt == null)
        {
            Console.WriteLine("Nie znaleziono produktu");
            return;
        }

        Console.WriteLine("Nowa nazwa:");
        string nazwa = Console.ReadLine();
        double cena = WczytajCene();
        int ilosc = WczytajIlosc();

        produkt.Nazwa = nazwa;
        produkt.Cena = cena;
        produkt.Ilosc = ilosc;

        Console.WriteLine("Zmieniono dane produktu");
        OdswiezListe();
    }

    static void UsunProdukt()
    {
        Console.WriteLine("Podaj ID produktu:");
        if (!int.TryParse(Console.ReadLine(), out int Id))
        {
            Console.WriteLine("Nie ma takiego ID!");
            return;
        }

        var produkt = produkty.Find(p => p.Id == Id);
        if (produkt == null)
        {
            Console.WriteLine("Nie znaleziono produktu");
            return;
        }

        Console.WriteLine("Czy napewno chcesz usunąć? (tak/nie)");
        string odpowiedz = Console.ReadLine().Trim().ToLower();
        if (odpowiedz == "tak")
        {
            produkty.Remove(produkt);
            Console.WriteLine("Usunięto produkt");
            OdswiezListe();
        }
    }

    static void ZapiszDoCsv()
    {
        using (var sw = new StreamWriter(filePath))
        {
            sw.WriteLine("Id,Nazwa,Cena,Ilosc");
            foreach (var p in produkty)
            {
                sw.WriteLine($"{p.Id},{p.Nazwa},{p.Cena},{p.Ilosc}");
            }
        }
        MessageBox.Query("Zapisano", $"Dane zapisane do {filePath}", "OK");
    }

    static void WczytajZCsv()
    {
        if (!File.Exists(filePath)) return;
        var lines = File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');
            produkty.Add(new Lista
            {
                Id = int.Parse(cols[0]),
                Nazwa = cols[1],
                Cena = double.Parse(cols[2]),
                Ilosc = int.Parse(cols[3])
            });
        }
    }

    // GUI Metody:
    static void OdswiezListe()
    {
        if (listView != null)
        {
            listView.SetSource(produkty.Select(p => p.ToString()).ToList());
        }
    }

    static void DodajProduktGUI()
    {
        var dialog = new Dialog("Dodaj Produkt", 60, 20);
        var nazwaField = new TextField("") { X = 1, Y = 1, Width = 40 };
        var cenaField = new TextField("") { X = 1, Y = 3, Width = 40 };
        var iloscField = new TextField("") { X = 1, Y = 5, Width = 40 };

        dialog.Add(new Label("Nazwa:") { X = 1, Y = 0 });
        dialog.Add(nazwaField);
        dialog.Add(new Label("Cena:") { X = 1, Y = 2 });
        dialog.Add(cenaField);
        dialog.Add(new Label("Ilość:") { X = 1, Y = 4 });
        dialog.Add(iloscField);

        var ok = new Button("OK");
        ok.Clicked += () =>
        {
            try
            {
                int noweID = produkty.Count == 0 ? 1 : produkty.Max(p => p.Id) + 1;
                string nazwa = nazwaField.Text.ToString();
                double cena = double.Parse(cenaField.Text.ToString().Replace('.', ','));
                int ilosc = int.Parse(iloscField.Text.ToString());

                produkty.Add(new Lista { Id = noweID, Nazwa = nazwa, Cena = cena, Ilosc = ilosc });
                OdswiezListe();
                Application.RequestStop();
            }
            catch
            {
                MessageBox.ErrorQuery("Błąd", "Nieprawidłowe dane wejściowe", "OK");
            }
        };

        var cancel = new Button("Anuluj");
        cancel.Clicked += () => Application.RequestStop();

        dialog.AddButton(ok);
        dialog.AddButton(cancel);

        Application.Run(dialog);
    }

    static void EdytujProduktGUI()
    {
        if (listView.SelectedItem < 0 || listView.SelectedItem >= produkty.Count)
            return;

        var produkt = produkty[listView.SelectedItem];

        var dialog = new Dialog("Edytuj Produkt", 60, 20);
        var nazwaField = new TextField(produkt.Nazwa) { X = 1, Y = 1, Width = 40 };
        var cenaField = new TextField(produkt.Cena.ToString()) { X = 1, Y = 3, Width = 40 };
        var iloscField = new TextField(produkt.Ilosc.ToString()) { X = 1, Y = 5, Width = 40 };

        dialog.Add(new Label("Nazwa:") { X = 1, Y = 0 });
        dialog.Add(nazwaField);
        dialog.Add(new Label("Cena:") { X = 1, Y = 2 });
        dialog.Add(cenaField);
        dialog.Add(new Label("Ilość:") { X = 1, Y = 4 });
        dialog.Add(iloscField);

        var ok = new Button("OK");
        ok.Clicked += () =>
        {
            try
            {
                produkt.Nazwa = nazwaField.Text.ToString();
                produkt.Cena = double.Parse(cenaField.Text.ToString().Replace('.', ','));
                produkt.Ilosc = int.Parse(iloscField.Text.ToString());
                OdswiezListe();
                Application.RequestStop();
            }
            catch
            {
                MessageBox.ErrorQuery("Błąd", "Nieprawidłowe dane wejściowe", "OK");
            }
        };

        var cancel = new Button("Anuluj");
        cancel.Clicked += () => Application.RequestStop();

        dialog.AddButton(ok);
        dialog.AddButton(cancel);

        Application.Run(dialog);
    }

    static void UsunProduktGUI()
    {
        if (listView.SelectedItem < 0 || listView.SelectedItem >= produkty.Count)
            return;

        var produkt = produkty[listView.SelectedItem];
        int n = MessageBox.Query("Usuń", $"Czy chcesz usunąć produkt: {produkt.Nazwa}?", "Tak", "Nie");
        if (n == 0)
        {
            produkty.Remove(produkt);
            OdswiezListe();
        }
    }
}
