using System.Timers;

namespace TableroEvts
{
    public partial class MainPage : ContentPage
    {
        public event EventHandler ReseteoTablero;
        private System.Timers.Timer _timer;

        public MainPage()
        {
            InitializeComponent();

            ReseteoTablero += OnReseteoTablero;

            _timer = new System.Timers.Timer(5000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private void OnResetButtonClicked(object sender, EventArgs e)
        {
            ReseteoTablero?.Invoke(this, EventArgs.Empty);
        }

        private void OnReseteoTablero(object sender, EventArgs e)
        {
            InputTextBox.Text = string.Empty;
            FocusTextBox.Text = string.Empty;
            HoverButton.BackgroundColor = Colors.LightGray;
            ClickButton.IsEnabled = true;
            TimerLabel.Text = "El temporizador no ha iniciado";
            EventLog.Text += "[Custom Event] Tablero reseteado.\n";

            // Opción para restablecer la posición del cuadro arrastrable
            DraggableBox.TranslationX = 0;
            DraggableBox.TranslationY = 0;
        }

        private void OnMouseEntered(object sender, FocusEventArgs e)
        {
            HoverButton.BackgroundColor = Colors.LightBlue;
            EventLog.Text += "[Mouse] Entró el mouse en el botón.\n";
        }

        private void OnMouseExited(object sender, FocusEventArgs e)
        {
            HoverButton.BackgroundColor = Colors.LightGray;
            EventLog.Text += "[Mouse] Salió el mouse del botón.\n";
        }

        private async void OnClickButtonClicked(object sender, EventArgs e)
        {
            ClickButton.IsEnabled = false;
            EventLog.Text += "[Click] Botón clickeado y deshabilitado temporalmente.\n";
            await Task.Delay(2000);
            ClickButton.IsEnabled = true;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CharCountLabel.Text = $"{InputTextBox.Text.Length} caracteres escritos";
            EventLog.Text += "[Keyboard] Se ha escrito texto.\n";
        }

        private void OnFocusTextBoxFocused(object sender, FocusEventArgs e)
        {
            EventLog.Text += "[Focus] Cuadro de texto enfocado.\n";
        }

        private void OnFocusTextBoxUnfocused(object sender, FocusEventArgs e)
        {
            EventLog.Text += "[Focus] Cuadro de texto desenfocado.\n";
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                TimerLabel.Text = $"Temporizador: {DateTime.Now:T}";
                EventLog.Text += $"[Timer] Actualización del temporizador: {DateTime.Now:T}\n";
            });
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var fontSize = e.NewValue;
            InputTextBox.FontSize = fontSize;
            FocusTextBox.FontSize = fontSize;
            EventLog.Text += $"[Slider] Tamaño de texto ajustado a: {fontSize:F0}.\n";
        }

        // Método para manejar el movimiento del cuadro arrastrable
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (sender is BoxView boxView)
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        EventLog.Text += "[Drag] Comenzó a arrastrar el cuadro.\n";
                        break;

                    case GestureStatus.Running:
                        // Obtener el tamaño del cuadro y las dimensiones de la pantalla
                        double boxWidth = boxView.Width;
                        double boxHeight = boxView.Height;
                        double maxWidth = Width - boxWidth;
                        double maxHeight = Height - boxHeight;

                        // Actualizar la posición del cuadro arrastrado con límites
                        boxView.TranslationX = Math.Max(0, Math.Min(maxWidth, boxView.TranslationX + e.TotalX));
                        boxView.TranslationY = Math.Max(0, Math.Min(maxHeight, boxView.TranslationY + e.TotalY));
                        break;

                    case GestureStatus.Completed:
                        EventLog.Text += "[Drag] Cuadro arrastrado a una nueva posición.\n";
                        break;
                }
            }
        }
    }
}
