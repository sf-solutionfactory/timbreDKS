using System;
using ServiceReference1;
using System.IO;
using System.ServiceModel;
using System.Text;

namespace DKS_Timbrar
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    //Obtener argumentos
                    string path = args[0].ToString();
                    string uname = args[1].ToString();
                    string passw = args[2].ToString();
                    string type = args[3].ToString();

                    //LLenar petición
                    ServiceReference1.stamp Stamp = new ServiceReference1.stamp();
                    Stamp.xml = File.ReadAllBytes(path);
                    Stamp.username = uname;
                    Stamp.password = passw;
                    stamp1 request = new stamp1(Stamp);
                    var myBinding = new BasicHttpBinding();

                    //Distinguir si es corrida en Calidad o Productivo
                    EndpointAddress myEndpoint;
                    if (type == "P")    //Productivo
                        myEndpoint = new EndpointAddress("https://facturacion.finkok.com/servicios/soap/stamp");
                    else                //Calidad
                        myEndpoint = new EndpointAddress("https://demo-facturacion.finkok.com/servicios/soap/stamp");


                    //LLamar WebService de FINKOK
                    ApplicationClient c = new ApplicationClient(new ApplicationClient.EndpointConfiguration(), myEndpoint);
                    ServiceReference1.stampResponse1 response = c.stamp(request);

                    //Si el XML fue correctamente timbrado
                    if (response.stampResponse.stampResult.UUID != null)
                    {
                        //Reemplazar XML con timbrado
                        File.WriteAllBytes(path, Encoding.ASCII.GetBytes(response.stampResponse.stampResult.xml));

                        //Regresar el UUID como mensaje
                        Console.WriteLine(response.stampResponse.stampResult.UUID);
                    }
                    else
                    {
                        //Si hubo algún error
                        foreach (ServiceReference1.Incidencia i in response.stampResponse.stampResult.Incidencias)
                        {
                            Console.WriteLine(i.MensajeIncidencia);
                            //var mess = Console.ReadLine();
                        }
                    }
                }
                catch (Exception e)
                {
                    //Mostrar mensaje de error
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
