using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using BruTile.Web;
using BruTile.Web.Wms;
using NUnit.Framework;
using Exception = System.Exception;

namespace BruTile.Tests.Web
{
    [TestFixture]
    internal class WmsCapabilitiesTest
    {
        [Test]
        public void WmsCapabilities_WhenSet_ShouldNotBeNull()
        {
            // arrange
            const string url = @"\Resources\CapabiltiesWmsC.xml";
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // act
            var capabilities = new WmsCapabilities(new Uri("file://" + directory + "\\" + url), null);

            // assert
            Assert.NotNull(capabilities.WmsVersion);
            var capability = capabilities.Capability;
            Assert.AreEqual(54, capability.Layer.ChildLayers.Count);
        }

        [Test]
        public void WmsCapabilities_SyntheticRoot()
        {
            // arrange
            const string url = @"\Resources\CapabilitiesWmsMultiTopLayers.xml";
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // act
            var capabilities = new WmsCapabilities(new Uri("file://" + directory + "\\" + url), null);

            // assert
            Assert.NotNull(capabilities.WmsVersion);
            var capability = capabilities.Capability;
            Assert.AreEqual("Root Layer", capability.Layer.Title);
            Assert.AreEqual(4, capability.Layer.ChildLayers.Count);
        }

        [Test, Ignore("Need to come up with something better, maybe http://www.searchogc.com")]
        public void TestSeveral()
        {
            var rnd = new Random();

            var urls = new List<Uri>
                           {
                               new Uri("http://wms.lizardtech.com/lizardtech/iserv/ows?REQUEST=GetCapabilities&SERVICE=WMS"),
                               new Uri(
                                   "http://www2.dmsolutions.ca/cgi-bin/mswms_gmap?REQUEST=GetCapabilities&SERVICE=WMS"),
                               new Uri("http://wms.geoimage.at/dop-1mfree?REQUEST=GetCapabilities&SERVICE=WMS"),
                               new Uri(
                                   "http://wms1.agr.gc.ca/cgi-bin/mapplant2000_f?service=wms&request=getCapabilities"),
                               new Uri(
                                   "http://wms1.agr.gc.ca/cgi-bin/mapplant1967_f?service=wms&request=getCapabilities"),
                               new Uri("http://wms1.agr.gc.ca/cgi-bin/mapquebec_en?service=wms&request=getCapabilities"),
                               new Uri(
                                   "http://80.198.124.155/ArealInfo/AI_WMS.asp?service=wms&VERSION=1.1.0&REQUEST=Getcapabilities"),

                               new Uri(
                                   "http://gis.vibamt.dk/ArealInfo/AI_WMS.asp?service=wms&VERSION=1.1.0&REQUEST=Getcapabilities"),
                               new Uri("http://wms1.agr.gc.ca/cgi-bin/mapquebec_fr?service=wms&request=getCapabilities"),
                               new Uri("http://132.156.10.87/cgi-bin/atlaswms_en?REQUEST=GetCapabilities&SERVICE=wms"),
                               new Uri("http://132.156.10.87/cgi-bin/atlaswms_en?VERSION=1.1.0&request=GetCapabilities&SERVICE=wms"),
                               new Uri("http://atlas.gc.ca/cgi-bin/atlaswms_en?VERSION=1.1.0&request=GetCapabilities"),

                               new Uri(
                                   "http://www.premis.cz/atlaszp/isapi.dll?MU=EN&SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri("http://www.wmap.cz/atlaszp/isapi.dll?MU=EN&SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri("http://194.228.3.80/atlaszp/isapi.dll?MU=CZ&SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://www.premis.cz/atlaszp/isapi.dll?MU=CZ&SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri("http://www.wmap.cz/atlaszp/isapi.dll?MU=CZ&SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri("http://www.wmap.cz/atlaszp/isapi.dll?SERVICE=WMS&MU=CZ&request=capabilities"),
                               new Uri(
                                   "http://155.187.2.28/ogcwms/servlet/com.esri.ogc.wms.WMSServlet?servicename=Map_maker&VERSION=1.1.1&SERVICE=WMS&request=GetCapabilities"),
                               new Uri(
                                   "http://audit.ea.gov.au/ogcwms/servlet/com.esri.ogc.wms.WMSServlet?servicename=Map_maker&VERSION=1.1.1&SERVICE=WMS&request=GetCapabilities"),
                               new Uri(
                                   "http://129.187.38.35/SICAD-IS60/isserver/ims/scripts/GetCapabilities.pl?request=GetCapabilities&Version=1.1.0&datasource=bplan&service=WMS"),
                               new Uri(
                                   "http://wms.gis.bv.tum.de/SICAD-IS60/isserver/ims/scripts/GetCapabilities.pl?request=GetCapabilities&Version=1.1.0&datasource=bplan&service=WMS"),

                               new Uri(
                                   "http://193.232.117.144/cgi-bin/mapserv?map=/var/www/html/mapFiles/maingis.map&request=GetCapabilities"),
                               new Uri(
                                   "http://grid1.wdcb.ru/cgi-bin/mapserv?map=/var/www/html/mapFiles/maingis.map&request=GetCapabilities"),
                               new Uri(
                                   "http://www.landesvermessung.sachsen.de/ias/basiskarte/service/SRVTKFREE/WMSFREE_TK/capabilities?datasource=atkis:de:WMSFREE_TK&REQUEST=capabilities"),
                               new Uri("http://deutschlandviewer.bayern.de/ogc/getogc.cgi?request=getcapabilities"),
                               new Uri(
                                   "http://205.150.58.109/cgi-bin/bsc_ows.asp?version=1.1.1&service=WMS&request=GetCapabilities"),

                               new Uri(
                                   "http://www.bsc-eoc.org/cgi-bin/bsc_ows.asp?version=1.1.1&service=WMS&request=GetCapabilities"),
                               new Uri(
                                   "http://wms.telascience.org/cgi-bin/ngBM_wms?Version=1.1.1&Service=WMS&Request=GetCapabilities"),
                               new Uri(
                                   "http://wms.telascience.org/cgi-bin/bm200401t_wms?Version=1.1.1&Service=WMS&Request=GetCapabilities"),
                               new Uri(
                                   "http://wms.telascience.org/cgi-bin/bm200401_wms?Version=1.1.1&Service=WMS&Request=GetCapabilities"),
                               new Uri(
                                   "http://wms.telascience.org/cgi-bin/bm200401tb_wms?Version=1.1.1&Service=WMS&Request=GetCapabilities"),

                               new Uri(
                                   "http://mapserv.netharvest.org/cgi-bin/mapserv?map=/www/mapserver/bulgaria.map&SERVICE=WMS&VERSION=1.0.0&request=getcapabilities"),
                               new Uri(
                                   "http://cgns.nrcan.gc.ca/wms/cubeserv.cgi?version=1.1.1&service=wms&request=GetCapabilities"),
                               new Uri("http://132.156.97.59/cgi-bin/canmin_en-ca_ows?request=GetCapabilities"),
                               new Uri("http://apps1.gdr.nrcan.gc.ca/cgi-bin/canmin_en-ca_ows?request=GetCapabilities"),
                               new Uri("http://mds.glc.org/cgi-bin/carolwms?VERSION=1.1.1&REQUEST=GetCapabilities"),

                               new Uri("http://mds.glc.org/cgi-bin/carolwms?request=getcapabilities"),
                               new Uri("http://132.156.97.3/cgi-bin/cgkn_wms?request=getcapabilities"),
                               new Uri("http://cgkn.net/cgi-bin/cgkn_wms?request=getcapabilities"),
                               new Uri("http://198.165.106.253/scripts/mapman.dll?Name=weather&request=GetCapabilities"),
                               new Uri(
                                   "http://geo.compusult.net/scripts/mapman.dll?Name=weather&request=GetCapabilities"),

                               new Uri(
                                   "http://divenos.meraka.csir.co.za:8080/geoserver/wms?service=wms&version=1.1.1&request=GetCapabilities"),
                               new Uri(
                                   "http://demo.cubewerx.com/demo/cubeserv/cubeserv.cgi?SERVICE=wms&VERSION=1.1.0&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://demo.cubewerx.com/mpp1/cubeserv/cubeserv.cgi?version=1.1.0&service=wms&request=GetCapabilities"),
                               new Uri("http://206.14.214.198/image?REQUEST=GetCapabilities"),
                               new Uri("http://maps.customweather.com/image?REQUEST=GetCapabilities"),

                               new Uri("http://maps.customweather.com/image?REQUEST=GetCapabilities&service=WMS"),
                               new Uri(
                                   "http://webapps.datafed.net/dvoy_services/ogc.wsfl?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://131.220.126.148:8080/deegree/wms?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://www.metoc.gov.au:8080/wmsconnector/com.esri.wms.Esrimap?REQUEST=GetCapabilities&SERVICE=WMS&"),
                               new Uri(
                                   "http://167.21.84.15/wmsconnector/com.esri.wms.Esrimap/DE_aerial02?service=WMS&version=1.1.1&request=getcapabilities"),

                               new Uri(
                                   "http://datamil.delaware.gov/wmsconnector/com.esri.wms.Esrimap/DE_aerial02?service=WMS&version=1.1.1&request=getcapabilities"),
                               new Uri(
                                   "http://www.gis.nrw.de/wms/DGM50?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri("http://195.27.54.43/wms/dlk/request.asp?request=GetCapabilities"),
                               new Uri(
                                   "http://209.217.116.146/cgi-bin/mswms_gmap?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://www2.dmsolutions.ca/cgi-bin/mswms_gmap?Service=WMS&VERSION=1.1.0&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://217.7.119.113/cgi-bin/proxy?USER_ID=GAST12345678&SERVICE=WMS&REQUEST=GetCapabilities&map=wms/DNM100n.map"),
                               new Uri(
                                   "http://199.212.16.10/envdat/map.aspx?service=WMS&version=1.1.1&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ns.ec.gc.ca/envdat/map.aspx?service=WMS&version=1.1.1&request=GetCapabilities"),
                               new Uri(
                                   "http://excise.pyr.ec.gc.ca/cgi-bin/mapserv.exe?map=/LocalApps/Mapsurfer/PYRWQMP.map&version=1.1.1&service=WMS&request=GetCapabilities"),
                               new Uri("http://193.204.228.31/cubestor/cubeserv/cubeserv.cgi?Request=GetCapabilities"),

                               new Uri(
                                   "http://193.204.228.31/cubestor/cubeserv/cubeserv.cgi?VERSION=1.1.1&REQUEST=GetCapabilities&SERVICE=WMS"),
                               new Uri(
                                   "http://mapserv2.esrin.esa.it/cubestor/cubeserv/cubeserv.cgi?REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://66.48.64.115/wmsconnector/com.esri.wsit.WMSServlet/Geobase_NRN_NewfoundlandAndLabrador_I_Detail?request=GetCapabilities"),
                               new Uri(
                                   "http://hazards.fema.gov/wmsconnector/Servlet/flood?REQUEST=GetCapabilities&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geographynetwork.ca/wmsconnector/com.esri.wsit.WMSServlet/Geobase_NRN_NewfoundlandAndLabrador_I_Detail?request=GetCapabilities"),

                               new Uri(
                                   "http://193.232.117.144/cgi-bin/mapserv?map=/var/www/html/mapFiles/mapFault.map&request=GetCapabilities"),
                               new Uri(
                                   "http://grid1.wdcb.ru/cgi-bin/mapserv?map=/var/www/html/mapFiles/mapFault.map&request=GetCapabilities"),
                               new Uri("http://clearinghouse1.fgdc.gov/scripts/ogc/ms.pl?request=capabilities"),
                               new Uri(
                                   "http://193.159.218.170/wmsconnector/wms/stobo?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.gis2.nrw.de/wmsconnector/wms/stobo?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),

                               new Uri("http://198.96.62.209/wms/cascader?REQUEST=getCapabilities&service=wms"),
                               new Uri(
                                   "http://gp2.chs-shc.dfo-mpo.gc.ca/wms/cascader?REQUEST=getCapabilities&service=wms"),
                               new Uri(
                                   "http://132.156.88.15/wmsconnector/com.esri.wms.Esrimap/energy_e?SERVICE=WMS&REQUEST=GetCapabilities&"),
                               new Uri(
                                   "http://gdr.ess.nrcan.gc.ca/wmsconnector/com.esri.wms.Esrimap/energy_e?SERVICE=WMS&REQUEST=GetCapabilities&"),
                               new Uri(
                                   "http://132.156.88.15/wmsconnector/com.esri.wms.Esrimap/GDR_E?SERVICE=WMS&REQUEST=GetCapabilities&"),

                               new Uri(
                                   "http://gdr.ess.nrcan.gc.ca/wmsconnector/com.esri.wms.Esrimap/GDR_E?SERVICE=WMS&REQUEST=GetCapabilities&"),
                               new Uri(
                                   "http://gdr.ess.nrcan.gc.ca/wmsconnector/com.esri.wms.Esrimap/GDR_F?SERVICE=WMS&REQUEST=GetCapabilities&"),
                               new Uri(
                                   "http://gdr.ess.nrcan.gc.ca/wmsconnector/com.esri.wms.Esrimap/geochron?SERVICE=WMS&REQUEST=GetCapabilities&"),
                               new Uri("http://wms.geodan.nl/startdata/sclmapserver.exe?request=Getcapabilities"),
                               new Uri(
                                   "http://193.109.138.13/cgi-bin/mapserv?VERSION=1.1.0&REQUEST=GetCapabilities&SERVICE=WMS"),

                               new Uri(
                                   "http://grid1.wdcb.ru/cgi-bin/mapserv?map=/var/www/html/mapFiles/geology.map&request=GetCapabilities"),
                               new Uri("http://www-a.ga.gov.au/bin/getmap.pl?request=capabilities"),
                               new Uri("http://www.ga.gov.au/bin/getmap.pl?dataset=national&request=capabilities"),
                               new Uri("http://www.ga.gov.au/bin/getmap.pl?dataset=national&request=getCapabilities"),
                               new Uri("http://www.ga.gov.au/bin/getmap.pl?request=capabilities"),

                               new Uri(
                                   "http://vesta.cast.uark.edu/wmscast/servlet/wmsesri?WMTVER=1.0.0&REQUEST=capabilities"),
                               new Uri(
                                   "http://wms1.ccgis.de/cgi-bin/mapserv?map=/data/umn/germany/germany.map&&VERSION=1.1.1&REQUEST=GetCapabilities&SERVICE=WMS"),
                               new Uri(
                                   "http://www.gis2.nrw.de/wmsconnector/wms/gewstat?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://195.113.207.44/cgi-bin/mapserv.exe?map=/gis/projekty/vysocina/vysocina.map&request=getCapabilities"),
                               new Uri(
                                   "http://195.113.207.44/cgi-bin/mapserv.exe?map=/gis/projekty/vysocina/vysocina_wms.map&request=getCapabilities"),

                               new Uri(
                                   "http://mapy.kr-vysocina.cz/cgi-bin/mapserv.exe?map=/gis/projekty/vysocina/vysocina.map&request=getCapabilities"),
                               new Uri(
                                   "http://mapy.kr-vysocina.cz/cgi-bin/mapserv.exe?map=/gis/projekty/vysocina/vysocina_wms.map&request=getCapabilities"),
                               new Uri("http://wms.globexplorer.com/gexservlets/wms?REQUEST=GetCapabilities"),
                               new Uri("http://www.gworks.ca/site/lib/wms/simple_wms.php?REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://195.243.84.83/cgi-bin/mapserv.exe?map=/web/wms/hgn-ogc.map&version=1.0.0&request=getCapabilities"),

                               new Uri(
                                   "http://wms.telascience.org/cgi-bin/katrina_ows?Version=1.1.1&Service=WMS&Request=GetCapabilities"),
                               new Uri(
                                   "http://wms.telascience.org/cgi-bin/hurricane_ows?Version=1.1.1&Service=WMS&Request=GetCapabilities"),
                               new Uri("http://128.40.25.70/cgi-bin/wms?map=wms.map&SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://iceds.ge.ucl.ac.uk/cgi-bin/icedswms?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://iceds.ge.ucl.ac.uk/cgi-bin/wms?map=wms.map&SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri("http://wms.larioja.org/request.asp?request=getcapabilities"),
                               new Uri("http://130.88.200.176/ecwp/ecw_wms.dll?request=GetCapabilities&service=wms"),
                               new Uri("http://camber.mc.man.ac.uk/ecwp/ecw_wms.dll?request=GetCapabilities&service=wms"),
                               new Uri(
                                   "http://www.earthetc.com/ecwp/ecw_wms.dll?request=GetCapabilities&amp;service=wms</Abstract>"),
                               new Uri("http://www.earthetc.com/ecwp/ecw_wms.dll?request=GetCapabilities&service=wms"),

                               new Uri(
                                   "http://maps1.intergraph.com/wms/ussample/request.asp?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://maps1.intergraph.com/wms/world/request.asp?service=WMS&request=GetCapabilities"),
                               new Uri("http://katalog.lgrb.de/cgi-bin/mapserv?REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://www.gis2.nrw.de/wmsconnector/wms/linfos?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.indexgeo.com.au/cgi-bin/wms-location?request=GetCapabilities&service=WMS"),

                               new Uri(
                                   "http://www.landesvermessung.sachsen.de/ias/basiskarte/service/SRVDOPFREE/WMSFREE_TK/wmsservice?REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://webmap.em.gov.bc.ca/liteview6.5/servlet/MapGuideLiteView?VERSION=1.1.1&Request=GetCapabilities"),
                               new Uri(
                                   "http://www.mapserver.niedersachsen.de/freezoneogc/mapserverogc?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://sidp.mapshed.com.au/wms/request?SERVICE=WMS&VERSION=1.1.3&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://terraserver.microsoft.com/ogccapabilities.ashx?version=1.1.1&amp;request=getcapabilities&amp;service=wms</font></p>"),

                               new Uri(
                                   "http://terraserver.microsoft.com/ogccapabilities.ashx?version=1.1.1&request=getcapabilities&service=wms"),
                               new Uri(
                                   "http://camber.mc.man.ac.uk/cgi-bin/mapserv.exe?map=c:%5CInetpub%5Cwwwroot%5CMIMAS%5Cwms_mimas_mosaic.map&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://156.98.35.213/cgi-bin/wms?map=DELI_WMS_MAPFILE&service=wms&version=1.1.1&request=GetCapabilities"),
                               new Uri(
                                   "http://deli.dnr.state.mn.us/cgi-bin/wms?map=DELI_WMS_MAPFILE&service=wms&version=1.1.1&request=GetCapabilities"),
                               new Uri("http://activefiremaps.fs.fed.us/wms/wms.asp?REQUEST=GetCapabilities"),

                               new Uri("http://www.hazardmaps.gov/wmsRequest.php?request=GetCapabilities"),
                               new Uri("http://test.chronos.org:9090/geoserver/wms?request=GetCapabilities&service=WMS"),
                               new Uri(
                                   "http://aes.gsfc.nasa.gov/cgi-bin/wms?%20SERVICE=WMS&VERSION=1.1.0&REQUEST=GetCapabilities"),
                               new Uri("http://nationalatlas.gov/natlas/capabilities.xml"),
                               new Uri(
                                   "http://mcmcwebmap.usgs.gov/OGCConnector/servlet/OGCConnector?&REQUEST=GetCapabilities&ServiceName=NDOP"),

                               new Uri(
                                   "http://193.159.218.170/wmsconnector/wms/hangneigung?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.gis2.nrw.de/wmsconnector/wms/hangneigung?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://grid1.wdcb.ru/cgi-bin/mapserv?map=/var/www/html/mapFiles/mapNLT.map&request=GetCapabilities"),
                               new Uri(
                                   "http://web.apps.state.nd.us/wmsconnector/com.esri.wms.Esrimap?VERSION=1.1.1&REQUEST=GetCapabilities&SERVICE=WMS&SERVICENAME=NDWMS_GeneralInfo&"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Abilene-Taylor_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/AccuWeather_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ada_County_ID_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Aiken_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alabama_DOT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alabama_Geological_Survey.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alachua_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alamance_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alaska_DFG_Commercial_Fisheries.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alaska_DFG_Sport_Fish_Division.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alaska_DFG_Wildlife_Conservation.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Albuquerque_NM_ArcIMS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Albuquerque_NM_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alexander_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alexandria_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Allegany_County_MD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Allegheny_County_PA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Allen_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alternative_Fuels_Data_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Alvin_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ameregis_Corp_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Amherst_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Anchorage_AK_City_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Anchorage_AK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Anderson_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Anson_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Apache_County_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Arapahoe_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ardmore_OK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Arizona_DOT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Arizona_SCO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Arkansas_CAST_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Arkansas_CAST_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Arkansas_Educational_Facilities.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Arlington_Heights_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/ARNG_Geographic_Data_Server.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Asheville_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Athens_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Atlanta_GA_Regional_Commission.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Atlantic_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Augusta_County_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Augusta_GA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Austin_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Australia_Fire_Sentinel_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Azusa_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Bakersfield_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Baltimore_MD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Baltimore_MD_Police_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Beaufort_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Bee_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Bergen_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Berkeley_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Bernalillo_County_NM_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Birmingham_AL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Black_Hawk_County_IA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Boca_Raton_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Boise_ID_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Boone_County_KY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Boston_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Boulder_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Bozeman_MT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Brewster_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/British_Columbia_Canada_Disease_Control.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/British_Columbia_Canada_Fisheries.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/British_Geological_Survey_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Brookline_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Brooks_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Broome_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Broomfield_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Broward_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Brunswick_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Bryan_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Buncombe_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Buncombe_County_NC_Sewerage_District.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Butler_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/C_and_S_Engineers_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cabarrus_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cabell_County_WV_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Calaveras_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/California_DOT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cambridge_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Camden_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cameron_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Canada_Natural_Resources_Earth_Sciences.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cape_May_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Carbon_County_UT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Carrboro_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Carson_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Carson_CA_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Carter_County_OK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cascade_County_MT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Catawba_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Central_Connecticut_State_University.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Charleston_County_SC_ArcIMS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Charleston_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Charlotte_County_FL_GIS.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Chelsea_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cherokee_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Chesapeake_Bay_Program.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Chicago_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Chicago_IL_Public_Schools_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Chorley_Borough_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Chula_Vista_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Chur_Switzerland_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Clallam_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Clark_County_NV_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Clark_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Clarksville_TN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Clinton_County_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Clovis_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Coconino_County_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Coeur_D%27Alene_Tribe_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Collier_County_FL_Appraiser_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Collier_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Colorado_Dept_of_Education.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Colorado_Dept_of_Public_Health_and_Environment.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Colorado_NDIS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Colorado_Springs_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Columbus_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Compass_Rose.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Concord_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Condor_Earth_Tech_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Connecticut_CLEAR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Connecticut_MAGIC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Conservation_International_CIGIS.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cookeville_TN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Corpus_Christi_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cranberry_Township_PA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Craven_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Culver_City_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Cumberland_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Dare_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/David_Rumsey_Collection.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Davie_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Davis_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Delaware_DNR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Delaware_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Delaware_River_Basin_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Denton_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Denton_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Des_Moines_IA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/DeSoto_County_MS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Devon_County_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Dimmit_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Door_County_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Douglas_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Downey_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Dunedin_New_Zealand_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/DuPage_County_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Durango-La_Plata_County_CO.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Durham_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Dutchess_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Duval_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Eagle_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Eagle_Technology_NZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Earth_Etc_Imagery.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Earth_Satellite_Corp_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/East_Riding_County_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Edwards_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ehime_Prefecture_Japan.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/El_Paso_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/EPA_EMaps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/EPA_EMaps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Erie_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Escambia_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Escambia_County_FL_Property_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/ESRI_California_Hawaii_Nevada_Office.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/ESRI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Euless_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Evansville_IN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fairbanks_North_Star_AK.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fairfax_County_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Falmouth_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Falmouth_ME_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fargo_ND_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fayetteville_AR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/FEMA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/FEMA_MMI_Hazard_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Finney_County_KS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Flathead_County_MT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fort_Bend_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fort_Collins_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fort_Lauderdale_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fort_Smith_AR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fort_Smith_AR_Police_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Franklin_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Frederick_County_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fresno_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Fresno_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Frio_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/GA_Central_Savannah_River_Area.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Garland_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Gaston_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Gateshead_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geauga_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Genesee_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geneva_Switzerland_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/GeoChron.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/GeoCommunicator_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geocortex_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geography_Network.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geography_Network_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geography_Network_Canada.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geography_Network_New_Zealand.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Geography_Network_Sweden.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/GeoMAC_Wildfire_Information_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Georgetown_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Georgia_Planning_Dept_of_Community_Affairs.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/GIDBImageServer.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/GIT_Civil_War_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Glastonbury_CT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Grand_Rapids_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Grapevine_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Grays_Harbor_County_WA.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Greene_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Greensboro_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Greenville_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Greenville_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Greenwood_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Gresham_OR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Grid_Lines.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Groton_CT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Guilford_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Halliburton_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hamburg_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hamilton_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hamilton_County_TN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hampton_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Harnett_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Harris_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Harrisonburg_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hart_Electric_Membership_Corp.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hawaii_Dept_of_Agriculture.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hawaii_Dept_of_Education.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hawaii_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hayward_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Henderson_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hidalgo_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/High_Point_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hillsborough_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hillsborough_County_FL_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hollywood_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hong_Kong_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Honolulu_HI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Horry_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hot_Springs_AR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Houston_TX_Bayou_Preservation.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Houston_TX_PWE_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Houston_TX_PWE_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Houston-Galveston_Area_Council_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Houston-Galveston_Area_Council_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hudspeth_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Huntington_County_IN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Hutt_City_New_Zealand_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Idaho_Falls_ID_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Idaho_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Idaho_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Indiana_Geological_Survey.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Indianapolis-Marion_County_IN.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Iowa_DNR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Iowa_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Iowa_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Iredell_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Irving_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Israel_eMaps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jackson_County_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jackson_County_MS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jackson_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jacksonville_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/James_City_County_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Japan_Landslide_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jasper_County_MO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jefferson_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jefferson_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jefferson_Davis_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jim_Hogg_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Jim_Wells_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Johnson_County_IA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kansas_KGS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kansas_KGS_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kansas_KGS_Maps_3.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kelowna_BC_Canada_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kenai_Peninsula_AK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kenosha_County_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kent_County_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kentucky_Geo_Net_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kentucky_Geological_Survey.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kentucky_Mines_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kentucky_Natural_Resources.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kentucky_Transportation_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/King_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kingston_Borough_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kinney_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kirkwood_MO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kitchener_ON_Canada_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kitsap_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Klickitat_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Knoxville_TN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kurikka_Finland_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Kuusamo_Finland_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/La_Plata_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/La_Salle_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lafayette_LA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Laguna_Beach_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lake_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lake_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lake_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lake_Tahoe_TIIMS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lakewood_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lambton_County_ON_Canada.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lancashire_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lane_County_OR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Laramie_County_WY_GIS.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Larimer_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Las_Vegas_NV_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/LaSalle_ON_Canada_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lawrence_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lebanon_NH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lee_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lenexa_KS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lenoir_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lewiston_ME_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lexington_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lexington-Fayette_County_KY.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lincoln_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lincoln_Parish_LA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Linn_County_IA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Little_Rock_AR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Livonia_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Los_Angeles_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Los_Angeles_County_Assessor_CA.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Los_Angeles_County_CA.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisiana_DEQ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisiana_Division_of_Administration.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisiana_DNR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisiana_DOTD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisiana_DOTD_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisiana_MAP.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisiana_National_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Louisville_KY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Loveland_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Lufkin_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Luzern_Canton_Switzerland.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Macon_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Madison_County_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mahoning_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maine_DOT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maine_MEGIS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Manatee_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maricopa_County_AZ_MAG_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maricopa_County_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Marietta_GA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Marin_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Marion_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Marshall_County_AL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Martin_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maryland_DOP_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maryland_Eastern_Shore_Regional_GIS.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maryland_MMRG_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Massachusetts_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Maverick_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/McMinnville_OR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mead_WA_School_District.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mecklenburg_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mecklenburg_County_NC_Property.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Melbourne_Australia_Port.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mercer_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Meridian_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mesa_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mesquite_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Miami-Dade_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Michael_Baker_Corp_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Michigan_SEMCOG_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mid_Willamette_Valley_OR_COG.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Midland_TX_Appraiser_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mifflin_County_PA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Milton_Ontario_Canada.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Milwaukee_Compass_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Milwaukee_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Minneapolis_Metro_Council_MN.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Minneapolis_MN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Minnehaha_County_SD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Minnesota_Department_of_Health.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Minnesota_Geoserver_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Minnesota_Legislature_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Minnesota_MDA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Missoula_County_MT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Missoula_MT_GCS_Research_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Missouri_CARES_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Missouri_Dept_of_Conservation.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Missouri_DHSS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Missouri_MCDC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mobile_AL_IMS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mohave_County_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Monmouth_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Monroe_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Monroe_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Montana_Cadastral_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Montana_GIAC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Montana_NRIS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Montgomery_County_MD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Montgomery_County_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Moore_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Mount_Pleasant_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/MSA_Professional_Services_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Museum_of_London_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nacogdoches_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Naperville_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NASA_ESAD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nash_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nashua_NH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nashville_TN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nashville_TN_Police_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/National_Geodetic_Survey.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/National_Oil_and_Gas_Assessment.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/National_Park_Service_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/National_Renewable_Energy_Lab.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/National_Undersea_Research_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/National_Wetlands_Inventory_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Navajo_County_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Neath_Port_Talbot_County_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nebraska_DNR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nebraska_GPC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Netherlands_RIVM_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nevada_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nevada_Legislature_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nevada_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Castle_County_DE_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Hampshire_GRANIT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Hanover_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Jersey_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Jersey_Meadowlands_Commission.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Jersey_MERI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Jersey_NJGIN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Jersey_NJGIN_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Mexico_Traffic_Safety_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Orleans_LA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_South_Wales_Australia_TopoWeb.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_York_City_Board_of_Education.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_York_City_Housing_and_Neighborhood_Info.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_York_DEC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_York_Travel_Info.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Zealand_Geological_and_Nuclear_Sciences.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/New_Zealand_NIWA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Newaygo_County_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Newfoundland_and_Labrador_Geological_Survey.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Newport_Beach_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Newton_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NGA_CADRG_DATA_UNITED_STATES_AND_TERRITORIES.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nidwalden_and_Obwalden_Cantons_Switzerland.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_Alaska_Fisheries_Science_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_Coastal_Protection_and_Restoration.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_Coastal_Services_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_Coral_Reef_Info_Systems.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_ENC_Direct_to_GIS.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_National_Ocean_Service.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_NGDC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_nowCOAST_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_Satellite_Services.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/NOAA_Shoreline_Extractor.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Norfolk_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Australian_Fire_Information.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_CGIA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_Coastal_Management.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_DENR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_DENR_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_e-NC_Authority_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_Eastern_COG_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_Flood_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_Soils_Explorer_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_Source_Water_Protection.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Carolina_Western_Piedmont_COG.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Dakota_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Myrtle_Beach_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/North_Vancouver_Canada_District_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Northern_Ireland_QUB_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Northern_Kentucky_LINK_GIS.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Northern_Ohio_NODIS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Norway_Geological_Survey.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nottingham_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nottinghamshire_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Nuclear_Terrorism_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Oakland_County_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ogden_UT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ohio_Dept_of_Job_and_Family_Services.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ohio_Dept_of_Natural_Resources.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ohio_Public_Utilities_Commission.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Okaloosa_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Oklahoma_City_OK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Oklahoma_OCGI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Onslow_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ontario_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ontario_Canada_Natural_Resources.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ontario_Canada_Snowmobile_Trails.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Orange_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Orange_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Orange_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Orangeburg_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Orem_UT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Osceola_County_FL_Property_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Overland_Park_KS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Overland_Park_KS_Stormwatch.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Oxford_County_Ontario_Canada.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pacific_Disaster_Center_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Palm_Beach_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pasquotank_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pecos_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pennsylvania_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pennsylvania_PPL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pennsylvania_WindMap.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Peoria_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Person_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Philadelphia_Citymaps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Philadelphia_PA_Crime_Base.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Phoenix_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pierce_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pike_County_PA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Plano_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Polk_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Port_St_Lucie_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Portage_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Portland_OR_Metro_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pottawattamie_County_IA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Presidio_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Prince_Georges_County_MD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Prince_William_County_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/ProMap_Corporation_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/ProMap_Corporation_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Providence_Plan_RI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Providence_RI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Public_Library_Geo_Database.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Pueblo_County_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Putnam_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Queensland_Natural_Resources_Australia.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Racine_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rapid_City_SD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/RBF_Consulting_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Reeves_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Regina_SK_Canada_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Reykjavik_Iceland_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rhode_Island_Environmental_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Richland_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Richmond_BC_Canada_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Richmond_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Richmond_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Richmond_KY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Riley_County_KS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Riverside_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Riverside_County_CA_Integrated_Project_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Roanoke_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rochester_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rock_Hill_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rock_Island_County_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rocky_Mount_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Roswell_GA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Round_Rock_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/RTI_Intl_Iraq_Portal.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rutgers_University_NJ_CRSSA.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Rutgers_University_NJ_CRSSA_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sacramento_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Salt_Lake_City_UT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sampson_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Antonio_TX_GIS.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Bernardino_CA_Associated_Governments.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Bernardino_CA_Water_Resources_Institute.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Bernardino_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Diego_CA_City_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Diego_REDI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Diego_SANDAG_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Francisco_Bay_Area_CA.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Francisco_CA_Traffic.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Francisco_CA_Transit.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Francisco_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Luis_Obispo_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/San_Patricio_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Santa_Cruz_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Santa_Fe_County_NM_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Santa_Monica_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sarasota_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/SC_Appalachian_COG_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/SC_Lower_Savannah_COG.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/SC_Midlands_COG_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Schaumburg_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Schneider_Corporation_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Schoharie_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Scotland_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Scotland_National_Library_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Scotland_RCAHMS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sedgwick_County_KS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sedona_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Seminole_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Seminole_County_FL_Property_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Shizuoka_Prefecture_Japan.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Smith_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Snohomish_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Somerset_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/South_Ayrshire_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Southwest_Watershed_Research_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Spartanburg_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Springfield_MA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Springfield_MO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/St_Louis_County_MO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/St_Louis_MO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/St_Louis_MO_Police_Dept.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/St_Marys_County_MD_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Starr_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Steamboat_Springs_CO_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Stockton_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Stony_Brook_GIS_Center_NJ.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Strategic_Consulting_Intl.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Suffolk_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Suffolk_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sugar_Land_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Summit_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sumter_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Surrey_County_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sussex_County_DE_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sussex_County_NJ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Sydney_Australia_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/TacomaSpace_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Tampa_Bay_FL_Area_Crime_Tracker.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Tennessee_CTAS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Terralogic_Inc_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/TerraServer.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Terrell_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Tewkesbury_Borough_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Texas_CEQ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Texas_CLEAR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Texas_DOT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Texas_NRIS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Texas_Tech_University_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Texas_WIID_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/The_Colony_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/The_National_Map.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/The_Nature_Conservancy_Protected_Areas.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Thurston_County_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/TIGER-Line_Data.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Timmons_Group_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Tippecanoe_County_IN_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Tompkins_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Troy_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Tulsa_Police_Dept_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/UK_DEAL_Oil_and_Gas_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/UK_Election_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/UK_Forestry_Commission.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/UK_Maps_Direct.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Union_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/United_Nations_Environment_Program.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Adelaide_Australia_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Adelaide_Australia_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Alabama_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Delaware_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Illinois_Chicago_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Memphis_Ground_Water_Institute.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Missouri_Space_Planning_and_Management.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Northern_Iowa_STORM.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/University_of_Texas_Arlington_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/US_Dept_of_Energy_Radiation_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/US_Fish_and_Wildlife_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/US_Health_Resources_and_Services.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USDA_Forest_Service_Geodata_Clearinghouse.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USDA_Forest_Service_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USDA_Forest_Service_Maps_2.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USDA_NRCS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USDA_Southern_Regional_Water_Quality.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USDA_Ventilation_Climate_Info.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Coastal_and_Marine_Geology.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Environmental_Mercury_Mapping.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Famine_Early_Warning_System.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Mid_Continent_Mapping_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_National_Wetlands_Research_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_New_York_Water_Resources.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Rocky_Mountain_Mapping_Center.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_S_Florida_SOFIA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Sagemap.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Status_Graphics.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Sustainable_Tree_Crops_Program.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/USGS_Western_Region_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Utah_County_UT_IMS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Utah_State_University_Water_Initiative.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Uvalde_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Vaasa_Finland_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Val_Verde_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Valdosta-Lowndes_County_GA.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Ventura_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Vermont_ANR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Vermont_VCGI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Vermont_VTrans_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Victoria_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Virginia_IMS_CCRM_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Visalia_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Visalia_CA_School_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Volusia_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Wake_County_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Walton_County_FL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Warren_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Warren_County_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washington_County_AR_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washington_County_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washington_DC_Area_Council_of_Governments.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washington_DC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washington_Dept_of_Ecology_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washington_Dept_of_Health_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washington_Dept_of_Transportation.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washoe_County_NV_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Washtenaw_County_MI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Waterloo_Region_Ontario_Canada.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Waukesha_County_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Waushara_County_WI_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Webb_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Weber_County_UT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/West_Hartford_CT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/West_Midlands_UK_Public_Health.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Westchester_County_NY_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Western_Australia_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Westerville_OH_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Wichita_KS_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Wide_Bay_Australia_Regional_Planning.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Will_County_IL_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Willacy_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Williamson_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Wilson_NC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Wisconsin_Legislature_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Woods_Hole_Oceanographic_Institute.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Woods_Hole_Oceanographic_Institute.wms?SERVICE=WMS&REQUEST=GetCapabilities&"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Worcestershire_UK_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Wyoming_Oil_and_Gas_Commission.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Yakima_WA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Yale_University_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Yavapai_County_AZ_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Yellowstone_County_MT_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Yolo_County_CA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/York_County_SC_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/York_County_VA_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Zapata_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://columbo.nrlssc.navy.mil/ogcwms/servlet/WMSServlet/Zavala_County_TX_Maps.wms?SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://193.159.218.174/wms/BK50?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/DGK5?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/DTK10?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://193.159.218.162/GeoOgcWms1.3/servlet/GEPNRW?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/NW2?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),

                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/TK100?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/TK25?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/TK50?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/NRW_Uebersicht?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.geoserver.nrw.de/GeoOgcWms1.3/servlet/NRW500?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),

                               new Uri(
                                   "http://142.176.62.108/cgi-bin/mapserv.exe?map=d:%5Cms441oci%5Cmaps%5CNS_ELECTIONS.map&wmtver=1.1.0&request=capabilities"),
                               new Uri(
                                   "http://142.176.62.108/cgi-bin/mapserv.exe?map=d:%5Cms441oci%5Cmaps%5CNS_TOPO_10000.map&wmtver=1.1.0&request=capabilities"),
                               new Uri("http://atlas.canri.nsw.gov.au/proxy/wms?request=capabilities"),
                               new Uri(
                                   "http://libcwms.gov.bc.ca/wmsconnector/com.esri.wsit.WMSServlet/ogc_layer_service?REQUEST=GetCapabilities&VERSION=1"),
                               new Uri(
                                   "http://www.gis-news.de/wms/getmapcap.php?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://www.gis-news.de/wms/getmapcap.php?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities&button2=GetCapabilityRequest"),
                               new Uri(
                                   "http://www.synbiosys.alterra.nl/wms/wms.asp?version=1.0.0&request=capabilities&WMS=Nederland"),
                               new Uri(
                                   "http://129.187.38.35/SICAD-IS60/isserver/ims/scripts/GetCapabilities.pl?request=GetCapabilities&Version=1.1.0&datasource=scalebar&service=WMS"),
                               new Uri(
                                   "http://wms.gis.bv.tum.de/SICAD-IS60/isserver/ims/scripts/GetCapabilities.pl?request=GetCapabilities&Version=1.1.0&datasource=scalebar&service=WMS"),
                               new Uri("http://atlas.gc.ca/cgi-bin/atlaswms_fr?VERSION=1.1.0&request=GetCapabilities"),

                               new Uri("http://nautilus.baruch.sc.edu/wms/seacoos_in_situ?REQUEST=GetCapabilities"),
                               new Uri("http://nautilus.baruch.sc.edu/wms/seacoos_rs?REQUEST=GetCapabilities"),
                               new Uri("http://nautilus.baruch.sc.edu/wms/seacoos_rs_256?REQUEST=GetCapabilities"),
                               new Uri("http://www.iderc.co.cu/Scripts/mapserver.exe?request=capabilities"),
                               new Uri(
                                   "http://sidp.mapwerks.com/cubeserv/cubeserv.cgi?REQUEST=GetCapabilities&version=1.1.1&service=wms"),

                               new Uri("http://kort.plandk.dk/scripts/mapserv.pl?service=wms&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://164.159.171.17/ogcwms/WmsServlet?service=wms&servicename=wafer&VERSION=1.1.1&REQUEST=getcapabilities"),
                               new Uri(
                                   "http://164.159.171.17/ogcwms/WmsServlet?servicename=nwi_wms&version=1.1.1&request=getcapabilities&service=wms"),
                               new Uri(
                                   "http://mapper.tat.fws.gov/ogcwms/WmsServlet?service=wms&servicename=crithab&VERSION=1.1.0&REQUEST=getcapabilities"),
                               new Uri(
                                   "http://mapper.tat.fws.gov/ogcwms/WmsServlet?service=wms&servicename=wafer_wms&VERSION=1.1.1&REQUEST=getcapabilities"),

                               new Uri(
                                   "http://globe.digitalearth.gov/viz-bin/wmt.cgi?VERSION=1.1.0&Request=GetCapabilities"),
                               new Uri("http://globe.digitalearth.gov/viz-bin/wmt.cgi?WMTVER=1.0.7&REQUEST=capabilities"),
                               new Uri("http://globe.digitalearth.gov/viz-bin/wmt.cgi?request=GetCapabilities"),
                               new Uri("http://viz.globe.gov/viz-bin/wmt.cgi?REQUEST=Capabilities"),
                               new Uri(
                                   "http://viz.globe.gov/viz-bin/wmt.cgi?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://viz.globe.gov/viz-bin/wmt.cgi?VERSION=&REQUEST=GetCapabilities&Service=WMS"),
                               new Uri("http://viz.globe.gov/viz-bin/wmt.cgi?WMTVER=1.1&REQUEST=capabilities"),
                               new Uri("http://nmcatalog.usgs.gov/catalogwms/base?request=getCapabilities"),
                               new Uri(
                                   "http://www.mapsherpa.com/cgi-bin/wms_iodra?SERVICE=wms&VERSION=1.1.1&REQUEST=getcapabilities"),
                               new Uri(
                                   "http://193.159.218.170/wmsconnector/wms/waldtyp?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),

                               new Uri(
                                   "http://www.gis2.nrw.de/wmsconnector/wms/waldtyp?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://193.159.218.170/wmsconnector/wms/wsg?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri(
                                   "http://www.gis2.nrw.de/wmsconnector/wms/wsg?REQUEST=GetCapabilities&VERSION=1.1.0&SERVICE=WMS"),
                               new Uri("http://www.bis.bayern.de/wms/gla/bfk_wms?request=getcapabilities&service=wms"),
                               new Uri(
                                   "http://203.202.1.216/proxy/dec/decdata?&VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities"),

                               new Uri(
                                   "http://ocs-spatial.ncd.noaa.gov/wmsconnector/com.esri.wms.Esrimap/encdirect?Service=WMS&Version=1.1.1&Request=GetCapabilities"),
                               new Uri(
                                   "http://doris.ooe.gv.at/wmsconnector/com.esri.wms.Esrimap?request=getcapabilities&service=wms"),
                               new Uri(
                                   "http://www.geoland.at/geolandWMS/service.aspx?Name=geoland_at_wms&Service=WMS&Version=1.1.1&Request=GetCapabilities"),
                               new Uri("http://www.bis.bayern.de/wms/gla/gk100_wms?request=getcapabilities&service=wms"),
                               new Uri(
                                   "http://205.156.4.85/wmsconnector/com.esri.wms.Esrimap/Surdex?service=wms&request=capabilities"),

                               new Uri(
                                   "http://ocs-spatial.ncd.noaa.gov/wmsconnector/com.esri.wms.Esrimap/Surdex?service=wms&request=capabilities"),
                               new Uri(
                                   "http://wetlandswms.er.usgs.gov/wmsconnector/com.esri.wms.Esrimap?ServiceName=USFWS_WMS_AK_Wetlands&Request=GetCapabilities&service=WMS"),
                               new Uri(
                                   "http://wetlandswms.er.usgs.gov/wmsconnector/com.esri.wms.Esrimap?ServiceName=USFWS_WMS_CONUS_Wetlands&Request=GetCapabilities&service=WMS"),
                               new Uri(
                                   "http://gisdata.usgs.net/servlet/com.esri.wms.Esrimap?REQUEST=GetCapabilities&SERVICE=wms"),
                               new Uri(
                                   "http://gisdata.usgs.net/servlet/com.esri.wms.Esrimap?REQUEST=GetCapabilities&amp;SERVICE=wms"),

                               new Uri(
                                   "http://gisdata.usgs.net/servlet/com.esri.wms.Esrimap?servicename=USGS_WMS_BTS_Roads&request=capabilities"),
                               new Uri(
                                   "http://gisdata.usgs.net/servlet/com.esri.wms.Esrimap?WMTVER=1.1.0&ServiceName=USGS_WMS_LANDSAT7&REQUEST=capabilities"),
                               new Uri(
                                   "http://gisdata.usgs.net/servlet/com.esri.wms.Esrimap?WMTVER=1.1.0&REQUEST=GetCapabilities&ServiceName=USGS_WMS_NED&Service=WMS"),
                               new Uri(
                                   "http://152.61.128.152/servlet/com.esri.wms.Esrimap?servicename=USGS_WMS_NHD&request=capabilities"),
                               new Uri(
                                   "http://152.61.128.152/servlet/com.esri.wms.Esrimap?servicename=USGS_WMS_NLCD&request=capabilities"),

                               new Uri(
                                   "http://gisdata.usgs.net/servlet/com.esri.wms.Esrimap?WMTVER=1.1.0&REQUEST=GetCapabilities&ServiceName=USGS_WMS_REF&Service=WMS"),
                               new Uri(
                                   "http://152.61.128.152/servlet/com.esri.wms.Esrimap/world?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://205.189.5.82/cgi-bin/mapserver/whc_ows.asp?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri("http://192.197.71.54/cgi-bin/mapeco?service=wms&request=getCapabilities"),
                               new Uri(
                                   "http://192.197.71.54/cgi-bin/mapeco?service=wms&version=1.1.0&request=GetCapabilities"),

                               new Uri("http://wms1.agr.gc.ca/cgi-bin/mapeco?service=wms&request=getCapabilities"),
                               new Uri(
                                   "http://wms1.agr.gc.ca/cgi-bin/mapeco?service=wms&version=1.1.0&request=GetCapabilities"),
                               new Uri("http://132.156.159.8/cgi-bin/cubeserv.cgi?VERSION=1.1.0&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://kreis-borken.map-server.de/wmsborken/gdi/wmsborken?Service=WMS&Version=1.1.1&REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://141.201.3.68/servlet/com.esri.wms.Esrimap?Version=1.1.1&Request=getcapabilities&Service=WMS"),

                               new Uri(
                                   "http://141.201.3.68/servlet/com.esri.wms.Esrimap?servicename=at_etm7_lambert&request=GetCapabilities"),
                               new Uri(
                                   "http://maps.geo.sbg.ac.at/servlet/com.esri.wms.Esrimap?Version=1.1.1&Request=getcapabilities&Service=WMS"),
                               new Uri(
                                   "http://maps.geo.sbg.ac.at/servlet/com.esri.wms.Esrimap?servicename=at_etm7_lambert&request=GetCapabilities"),
                               new Uri(
                                   "http://198.102.62.145/servlet/com.esri.wms.Esrimap?request=GetCapabilities&ServiceName=ESRI_Snow"),
                               new Uri(
                                   "http://203.11.121.53/servlet/com.esri.wms.Esrimap?VERSION=1.1.0&Request=getcapabilities"),

                               new Uri(
                                   "http://atlas.walis.wa.gov.au/servlet/com.esri.wms.Esrimap?VERSION=1.1.0&Request=getcapabilities"),
                               new Uri("http://atlas.walis.wa.gov.au/servlet/com.esri.wms.Esrimap?request=capabilities"),
                               new Uri(
                                   "http://dnweb5.dirnat.no/servlet/com.esri.wms.Esrimap?WMTVER=1.0&REQUEST=capabilities&ServiceName=WMS_NB_Arter"),
                               new Uri(
                                   "http://dnweb5.dirnat.no/servlet/com.esri.wms.Esrimap?WMTVER=1.0&REQUEST=capabilities&ServiceName=WMS_NB_Vern"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=Deck41&WMTVER=1.0&request=GetCapabilities"),

                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=Geolin&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=Hot_Springs&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=Sample_Index&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=baz&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=bec&WMTVER=1.0&request=GetCapabilities"),

                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=faosoil&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=firedetects&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=fishmap&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=fvvveg&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=glacier&WMTVER=1.0&request=GetCapabilities"),

                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=greatlakesbathy&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=hazards&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=hazards_tsunami&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=multibeam&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=nei99_v3&WMTVER=1.0&request=GetCapabilities"),

                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=osei&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://map.ngdc.noaa.gov/servlet/com.esri.wms.Esrimap?servicename=timeline&WMTVER=1.0&request=GetCapabilities"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?&REQUEST=capabilities"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?ServiceName=ESRI_World&WMTVER=1.0.0&request=capabilities"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&ServiceName=ESRI_Snow"),

                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&ServiceName=ESRI_Soil"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&ServiceName=ESRI_Veg"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_AgThreat"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Elev"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Land"),

                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Landuse"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Mineral"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Pop"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Precip_Yr"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Snow"),

                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Tmp_Yr"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Trans"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_Veg"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=ESRI_World"),
                               new Uri(
                                   "http://www.geographynetwork.com/servlet/com.esri.wms.Esrimap?request=GetCapabilities&amp;ServiceName=FEMA_Flood"),

                               new Uri("http://negeo.ne.ch/ogc-sitn/wms?REQUEST=Capabilities"),
                               new Uri(
                                   "http://www.geographie.uni-freiburg.de/cgi-bin/mapserv?map=/web/mapserver/kgis/WMS.map&service=WMS&version=1.1.0&request=getcapabilities"),
                               new Uri("http://geodaten.llv.li/WMS?SERVICE=WMS&REQUEST=GetCapabilities"),
                               new Uri("http://www.neonet.nl/servlet/WmsServlet?REQUEST=GetCapabilities"),
                               new Uri(
                                   "http://194.171.50.154/mapserver/request.asp?VERSION=&REQUEST=GetCapabilities&Service=WMS"),

                               new Uri("http://www2.demis.nl/WMS/wms.asp?WMS=WorldMap&WMTVER=1.0.0&request=capabilities"),
                               new Uri("http://www2.demis.nl/mapserver/Request.asp?REQUEST=Capabilities"),
                               new Uri(
                                   "http://www2.demis.nl/mapserver/request.asp?VERSION=&REQUEST=GetCapabilities&Service=WMS"),
                               new Uri(
                                   "http://www2.demis.nl/wms/wms.asp?wms=WorldMap&request=getcapabilities&version=1.0.7"),
                               new Uri("http://132.156.97.59/cgi-bin/worldmin_en-ca_ows?request=GetCapabilities"),

                               new Uri("http://apps1.gdr.nrcan.gc.ca/cgi-bin/worldmin_en-ca_ows?request=GetCapabilities"),
                               new Uri(
                                   "http://grid1.wdcb.ru/cgi-bin/mapserv?map=/var/www/html/mapFiles/wsm.map&request=GetCapabilities"),
                               new Uri(
                                   "http://services.interactive-instruments.de/xtra/cgi-bin/wms?REQUEST=GetCapabilities&SERVICENAME=wms"),
                               new Uri(
                                   "http://services.interactive-instruments.de/xtra/cgi-bin/wms?REQUEST=GetCapabilities&SERVICENAME=wms"),
                               new Uri(
                                   "http://services.interactive-instruments.de/xtra/cgi-bin/wms?REQUEST=GetCapabilities&SERVICENAME=wms"),
                           };

            var failed = 0;

            RequestHelper.Timeout = 1000;
            var tested = 0;

            while (tested < 25)
            {
                //Pick random from list
                var uri = urls[rnd.Next(0, urls.Count - 1)];

                var uriString = uri.OriginalString.ToLowerInvariant();
                var useUri = uri;
                if (!uriString.Contains("service=wms")) useUri = new Uri(uri.OriginalString + "&service=wms");

                Console.Write(useUri);
                try
                {
                    new WmsCapabilities(uri, RequestHelper.WebProxy);
                    Console.WriteLine("; ... passed");
                    tested++;
                }
                catch (WebException we)
                {
                    Console.WriteLine("; ... FAILED!");
                    Console.WriteLine(string.Format("\t{0}", we.Message));
                }

                catch (Exception e)
                {
                    var testCounts = true;
                    var wp = e as WmsParsingException;
                    if (e.Message.StartsWith("ServiceException")) testCounts = false;

                    else if (wp != null && (wp.Message.Contains("<Service>") || wp.Message.Contains("<Capability>")))
                        testCounts = false;

                    Console.WriteLine("; ... FAILED!");
                    while (e != null)
                    {
                        Console.WriteLine(string.Format("\t{0}", e.Message));
                        e = e.InnerException;
                    }

                    if (testCounts)
                    {
                        tested++;
                        failed++;
                    }
                }
            }

            Assert.AreEqual(0, failed, string.Format("WmsCapabilities parsing failed {0} times", failed));
        }
    }
}