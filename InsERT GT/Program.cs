using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsERT_GT
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                InsERT.GT gt = new InsERT.GT();
                gt.Produkt = InsERT.ProduktEnum.gtaProduktSubiekt;
                gt.Serwer = "";
                gt.Baza = "";
                gt.Autentykacja = InsERT.AutentykacjaEnum.gtaAutentykacjaMieszana;
                gt.Uzytkownik = "";
                gt.UzytkownikHaslo = "";
                gt.Operator = "";
                gt.OperatorHaslo = "";

                InsERT.Subiekt subiekt = (InsERT.Subiekt)gt.Uruchom((Int32)InsERT.UruchomDopasujEnum.gtaUruchomDopasuj,
                    (Int32)InsERT.UruchomEnum.gtaUruchomNieArchiwizujPrzyZamykaniu);
                subiekt.Okno.Widoczne = true;

                InsERT.SuDokument dok = subiekt.Dokumenty.Wczytaj(487);
                InsERT.SuPozycje dok_poz = dok.Pozycje;
                

                string strFilePath = @"C:\Users\Dominika\Desktop\cena1.csv";
                DataTable towary = ConvertCSVtoDataTable(strFilePath);
                DataTable tb = new DataTable();
                DataRow dr;

                tb.Columns.Add("Symbol", typeof(String));
                tb.Columns.Add("Nazwa", typeof(String));
                tb.Columns.Add("Cena Netto", typeof(float));
                tb.Columns.Add("Cena Netto1", typeof(float));

                Console.WriteLine("Liczba wczytanych rekordów: " + towary.Rows.Count);

                string slo = "falsz";
                int count = 0;

                foreach (DataRow row in towary.Rows)
                {
                    if (row["Status"].ToString().Equals(slo))
                    {
                        count++;
                        dr = tb.NewRow();
                        dr["Symbol"] = row["Symbol"];
                        dr["Nazwa"] = row["Nazwa"];
                        dr["Cena Netto"] = row["Cena Netto"];
                        dr["Cena Netto1"] = row["Cena Netto1"];
                        tb.Rows.Add(dr);
                    }

                }
                Console.WriteLine("Liczba nieprawidłowych danych w .csv: " + count);


                foreach (DataRow rt in tb.Rows)
                {
                    foreach (InsERT.SuPozycja poz in dok_poz)
                    {
                        if (rt["Symbol"].Equals(poz.TowarSymbol))

                        {

                            Console.WriteLine("Tak jest! " + poz.TowarSymbol);
                            poz.WartoscNettoPoRabacie = rt["Cena Netto1"];
                            poz.Opis = "ok";
                            

                        }
                    }
                }
                

                dok.Zapisz();
                dok.Zamknij();
                subiekt.Zakoncz();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            Console.ReadKey();
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(';');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(';');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }


            return dt;
        }
    }
}
