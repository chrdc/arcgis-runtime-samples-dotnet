// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Threading.Tasks;
using CoreGraphics;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using UIKit;

namespace ArcGISRuntime.Samples.RenderPictureMarkers
{
    [Register("RenderPictureMarkers")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Render picture markers",
        "Symbology",
        "This sample demonstrates how to create picture marker symbols from a URL and embedded resources.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.EmbeddedResource(@"PictureMarkerSymbols\pin_star_blue.png")]
    public class RenderPictureMarkers : UIViewController
    {
        // Create and hold a reference to the used MapView.
        private readonly MapView _myMapView = new MapView();

        public RenderPictureMarkers()
        {
            Title = "Render picture markers";
        }

        public override void ViewDidLoad()
        {
            // Create the UI, setup the control references and execute initialization.
            CreateLayout();
            Initialize();

            base.ViewDidLoad();
        }

        public override void ViewDidLayoutSubviews()
        {
            try
            {
                nfloat topMargin = NavigationController.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height;

                // Reposition controls.
                _myMapView.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
                _myMapView.ViewInsets = new UIEdgeInsets(topMargin, 0, 0, 0);

                base.ViewDidLayoutSubviews();
            }
            // Needed to prevent crash when NavigationController is null. This happens sometimes when switching between samples.
            catch (NullReferenceException)
            {
            }
        }

        private async void Initialize()
        {
            // Create new Map with topographic basemap.
            Map myMap = new Map(Basemap.CreateTopographic());

            // Create and set initial map area.
            Envelope initialLocation = new Envelope(-229835, 6550763, -222560, 6552021, SpatialReferences.WebMercator);
            myMap.InitialViewpoint = new Viewpoint(initialLocation);

            // Assign the map to the MapView.
            _myMapView.Map = myMap;

            // Create overlay to where graphics are shown.
            GraphicsOverlay overlay = new GraphicsOverlay();

            // Add created overlay to the MapView.
            _myMapView.GraphicsOverlays.Add(overlay);

            // Add graphics using different source types.
            CreatePictureMarkerSymbolFromUrl(overlay);
            await CreatePictureMarkerSymbolFromResources(overlay);
        }

        private void CreatePictureMarkerSymbolFromUrl(GraphicsOverlay overlay)
        {
            // Create URL to the image.
            var symbolUri = new Uri("https://sampleserver6.arcgisonline.com/arcgis/rest/services/Recreation/FeatureServer/0/images/e82f744ebb069bb35b234b3fea46deae");

            // Create new symbol using asynchronous factory method from URL.
            PictureMarkerSymbol campsiteSymbol = new PictureMarkerSymbol(symbolUri)
            {
                // Optionally set the size (if not set, the size in pixels of the image will be used).
                Height = 40,
                Width = 40
            };

            // Create location for the campsite.
            MapPoint campsitePoint = new MapPoint(-223560, 6552021, SpatialReferences.WebMercator);

            // Create graphic with the location and symbol.
            Graphic campsiteGraphic = new Graphic(campsitePoint, campsiteSymbol);

            // Add graphic to the graphics overlay.
            overlay.Graphics.Add(campsiteGraphic);
        }

        private async Task CreatePictureMarkerSymbolFromResources(GraphicsOverlay overlay)
        {
            // Get current assembly that contains the image.
            var currentAssembly = Assembly.GetExecutingAssembly();

            // Get image as a stream from the resources.
            // Picture is defined as EmbeddedResource and DoNotCopy.
            var resourceStream = currentAssembly.GetManifestResourceStream(
                "ArcGISRuntime.Resources.PictureMarkerSymbols.pin_star_blue.png");

            // Create new symbol using asynchronous factory method from stream.
            PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(resourceStream);
            pinSymbol.Width = 50;
            pinSymbol.Height = 50;

            // Create location for the pin.
            MapPoint pinPoint = new MapPoint(-226773, 6550477, SpatialReferences.WebMercator);

            // Create graphic with the location and symbol.
            Graphic pinGraphic = new Graphic(pinPoint, pinSymbol);

            // Add graphic to the graphics overlay.
            overlay.Graphics.Add(pinGraphic);
        }

        private void CreateLayout()
        {
            // Add MapView to the page.
            View.AddSubviews(_myMapView);
        }
    }
}