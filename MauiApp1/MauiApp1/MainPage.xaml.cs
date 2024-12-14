using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private int points = 0;
        private int pointsPerClick = 1;
        private int passivePoints = 50;
        private int clickUpgradeCost = 100;
        private int passiveUpgradeCost = 200;
        private bool canClaimPoints = true;

        public MainPage()
        {
            InitializeComponent();
            UpdateUI();
        }

        private async void OnPlanetTapped(object sender, EventArgs e)
        {
            int oldPoints = points;
            points += pointsPerClick;

            await SmoothUpdatePoints(oldPoints, points); // Płynne przejście licznika punktów
            await PlanetImage.ScaleTo(0.9, 50);          // Animacja kliknięcia planety
            await PlanetImage.ScaleTo(1, 50);
            await AnimatePointsLabel();                  // Animacja pulsowania punktów
        }

        private async void OnClaimPointsClicked(object sender, EventArgs e)
        {
            if (canClaimPoints)
            {
                int oldPoints = points;
                points += passivePoints;

                await SmoothUpdatePoints(oldPoints, points); // Płynne przejście punktów
                canClaimPoints = false;
                ClaimPointsButton.IsEnabled = false;
                await ShowNotification($"Dostałeś {passivePoints} punktów!");
                await StartClaimTimer();
                UpdateUI();
            }
        }

        private async Task StartClaimTimer()
        {
            for (int i = 10; i > 0; i--)
            {
                ClaimPointsButton.Text = $"Odbierz darmowe punkty ({i}s)";
                await Task.Delay(1000);
            }

            canClaimPoints = true;
            ClaimPointsButton.IsEnabled = true;
            ClaimPointsButton.Text = "Odbierz darmowe punkty";
        }

        private async void OnOpenUpgradesClicked(object sender, EventArgs e)
        {
            UpgradesPopup.Opacity = 0;
            UpgradesPopup.IsVisible = true;
            await UpgradesPopup.FadeTo(1, 300);
        }

        private async void OnCloseUpgradesClicked(object sender, EventArgs e)
        {
            await UpgradesPopup.FadeTo(0, 300);
            UpgradesPopup.IsVisible = false;
        }

        private async void OnBuyUpgradeClicked(object sender, EventArgs e)
        {
            if (points >= clickUpgradeCost)
            {
                points -= clickUpgradeCost;
                pointsPerClick += 1;
                clickUpgradeCost *= 2;

                UpdateUI();
                await ShowNotification("Punkty za klik zostały ulepszone!");
            }
            else
            {
                await ShowNotification("Za mało punktów na ulepszenie!");
            }
        }

        private async void OnBuyPassiveUpgradeClicked(object sender, EventArgs e)
        {
            if (points >= passiveUpgradeCost)
            {
                points -= passiveUpgradeCost;
                passivePoints += 50;
                passiveUpgradeCost *= 2;

                UpdateUI();
                await ShowNotification("Więcej punktów za nieaktywność zostało ulepszone!");
            }
            else
            {
                await ShowNotification("Za mało punktów na ulepszenie!");
            }
        }

        private async Task ShowNotification(string message)
        {
            NotificationMessage.Text = message;
            NotificationPopup.IsVisible = true;

            NotificationPopup.Opacity = 0;
            await NotificationPopup.FadeTo(1, 200); // Szybsza animacja
            await Task.Delay(1500); // Szybsze zamknięcie
            await HideNotification();
        }

        private async Task HideNotification()
        {
            await NotificationPopup.FadeTo(0, 200); // Szybsza animacja
            NotificationPopup.IsVisible = false;
        }

        private void UpdateUI()
        {
            PointsLabel.Text = $"Punkty: {points}";
            ClickUpgradeCostLabel.Text = $"Koszt: {clickUpgradeCost} pkt";
            PassiveUpgradeCostLabel.Text = $"Koszt: {passiveUpgradeCost} pkt";
        }

        private async Task AnimatePointsLabel()
        {
            await PointsLabel.ScaleTo(1.2, 100, Easing.CubicInOut); // Powiększenie
            await PointsLabel.ScaleTo(1.0, 100, Easing.CubicInOut); // Powrót do normalnego rozmiaru
        }

        private async Task SmoothUpdatePoints(int oldPoints, int newPoints)
        {
            for (int i = oldPoints; i <= newPoints; i++)
            {
                PointsLabel.Text = $"Punkty: {i}";
                await Task.Delay(1); // Opóźnienie między krokami
            }
        }

        private async void OnNotificationCloseClicked(object sender, EventArgs e)
        {
            await HideNotification();
        }
    }

}
