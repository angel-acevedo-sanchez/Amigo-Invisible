using System;
using System.Linq;
using System.Threading;
using System.IO;

namespace AmigoInvisibleCshark
{
    class Program
    {

        static void Main(string[] args)
        {

            int num_sending = num_lines("list.csv");

            string[] correosInicial = new string[num_sending];
            string[] correosEnvio = new string[num_sending];
            string[] nombres = new string[num_sending];
            string[] nombresFinal = new string[num_sending];
            int[] orden = new int[num_sending];
            int i;
            string[] msg = null;
            string line = " ";
            string[] field = null;


            for (i = 0; i < num_sending; i++) orden[i] = -1;


            i = 0;

            System.Net.Mail.MailMessage mensaje = new System.Net.Mail.MailMessage();

            mensaje.Subject = "Amigo Invisible";
            mensaje.SubjectEncoding = System.Text.Encoding.UTF8;
            mensaje.From = new System.Net.Mail.MailAddress("user@outlook.es");

            System.Net.Mail.SmtpClient clienteCorreo = new System.Net.Mail.SmtpClient();

            clienteCorreo.Credentials = new System.Net.NetworkCredential("user@outlook.es", "password");
            clienteCorreo.Host = "smtp.office365.com";
            clienteCorreo.EnableSsl = true;

            try
            {
                StreamReader sr = new StreamReader("list.csv"); //lista de nombres;correo de los participantes

                line = sr.ReadLine();

                while (line != null)
                {
                    field = line.Split(';');
                    nombres[i] = field[0];
                    correosInicial[i] = field[1];

                    i++;
                    line = sr.ReadLine();

                }

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);

            }


            ordenacion(correosInicial, correosEnvio, orden, nombres, nombresFinal); //función para realizar el algoritmo de emparejamiento


            for (i = 0; i < num_sending; i++)
            {


                try
                {
                    string path = "message.txt";
                    msg = File.ReadAllLines(path);


                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                string msg_final = String.Join(" ", msg);

                msg_final = msg_final.Replace("HERE", nombresFinal[(i + 1) % nombresFinal.Length]); 
                //En un posible fichero HTML se busca la palabra HERE para sustituirla por el nombre y asi personalizar el mensaje.

                mensaje.Body = msg_final;
                mensaje.IsBodyHtml = true;
                mensaje.BodyEncoding = System.Text.Encoding.UTF8;
                send_mail(correosEnvio[i], mensaje, clienteCorreo);

                Console.WriteLine("Enviado correo numero " + i);

            }

        }

        static void ordenacion(string[] a, string[] b, int[] c, string[] n, string[] nf)
        {

            Random aleatorio = new Random();

            int valor = aleatorio.Next(0, a.Length);

            c[0] = valor;
            b[0] = a[valor];
            nf[0] = n[valor];


            for (int i = 1; i < a.Length; i++)
            {

                while (c.Contains(valor))
                {
                    valor = aleatorio.Next(0, a.Length); //mientras que el valor generado se encuentra YA en el array, calculo otro --> quiero un valor diferente

                }
                c[i] = valor;
                b[i] = a[valor]; //en la lista de correos definitiva vamos ordenando los correos segun el valor aleatorio generado
                nf[i] = n[valor];
            }


        }

        static void send_mail(string destino, System.Net.Mail.MailMessage m, System.Net.Mail.SmtpClient c)
        {

            m.To.Add(destino);
            c.Send(m);

            Thread.Sleep(1000); //Los mail se mandan con un retardo de 1 segundo para evitar el bloqueo por SPAM
            m.To.Clear(); //limpiamos el destinatario

        }

        static int num_lines(string path)
        {
            String line = " ";
            int lines = 0;

            try
            {
                StreamReader sr = new StreamReader(path);

                line = sr.ReadLine();

                while (line != null)
                {
                    lines++;
                    line = sr.ReadLine();

                }

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return lines;

        }

    }
}
