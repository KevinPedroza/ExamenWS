using System;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Device.Location;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using GoogleDirections;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using System.Linq;

namespace ConsumirWS
{
    public partial class Form1 : Form
    {
        GMarkerGoogle marker = null;
        GMapOverlay mapOverlay = null;
        private GeoCoordinateWatcher Watcher = null;
        double latini;
        double lonini;
        public static Object lugar;
        public Form1()
        {
            InitializeComponent();
            latini = 10.3238;
            lonini = -84.4271;
        }
        string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
        public void RetrieveFormatedAddress(string lat, string lng)
        {
            string requestUri = string.Format(baseUri, lat, lng);

            using (WebClient wc = new WebClient())
            {
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.DownloadStringAsync(new Uri(requestUri));
            }
        }
        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var xmlElm = XElement.Parse(e.Result);

                var status = (from elm in xmlElm.Descendants()
                              where elm.Name == "status"
                              select elm).FirstOrDefault();
                var id = (from elm in xmlElm.Descendants()
                          where elm.Name == "place_id"
                          select elm).FirstOrDefault();
                if(id.Value != null)
                    lugar = id.Value;
                MessageBox.Show(lugar.ToString());
                if (status.Value.ToLower() == "ok")
                {
                    var locality = xmlElm.Descendants("type").FirstOrDefault(x => x.Value == "locality");
                    if (locality != null)
                    {
                        var long_name = locality.Parent.Element("long_name");

                        if (long_name.Value != null)
                        {
                            lu.Text = long_name.Value.ToString();
                        }
                    }
                    else
                    {
                        lu.Text = "Localizando...";
                    }

                }
            }
            catch (Exception err)
            {
            }
        }
        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Ready)
            {
                gMapControl1.Overlays.Clear();
                // Display the latitude and longitude.
                latini = Watcher.Position.Location.Latitude;
                lonini = Watcher.Position.Location.Longitude;
                cargarMapa(latini, lonini);
            }

        }
        private void cargarMapa(double latini, double lonini)
        {
            //Cargar Mapa
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(latini, lonini);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 9;
            gMapControl1.AutoScroll = true;
            //Marcador
            mapOverlay = new GMapOverlay("Marcador");
            marker = new GMarkerGoogle(new PointLatLng(latini, lonini), GMarkerGoogleType.green);
            mapOverlay.Markers.Add(marker);
            //Mostrador
            marker.ToolTipMode = MarkerTooltipMode.Always;
            marker.ToolTipText = string.Format("Ubicación : \n Latitud:{0} \n Longitud:{1}", latini, lonini);
            //Agregar a Mapa
            gMapControl1.Overlays.Add(mapOverlay);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Watcher = new GeoCoordinateWatcher();
            Watcher.StatusChanged += Watcher_StatusChanged;
            Watcher.Start();
            cargarMapa(latini, lonini);
        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {

            //obtener info y mostrar
            double lat = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;
            double ln = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;
            //mostrar la info
            latitud.Text = lat.ToString();
            longitud.Text = ln.ToString();
            //posicionar el marker
            marker.Position = new PointLatLng(lat, ln);
            //mostrar texto en tooltip
            marker.ToolTipText = String.Format("Ubicación: \n Latitud:{0} \n Longitud:{1}", lat, ln);

            string newlat = lat.ToString().Replace(",", ".");
            string newln = ln.ToString().Replace(",", ".");
            RetrieveFormatedAddress("10.3238","-84.4271");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MostrarProno mp = new MostrarProno();
            mp.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
