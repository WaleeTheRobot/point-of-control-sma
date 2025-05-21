using NinjaTrader.Data;
using NinjaTrader.NinjaScript.BarsTypes;
using PointOfControlMovingAverage;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;

namespace PointOfControlMovingAverage
{
    public enum BarTypeOptions
    {
        Minute,
        Range,
        Second,
        Tick,
        Volume
    }
}

namespace NinjaTrader.NinjaScript.Indicators
{
    public class PointOfControlMovingAverage : Indicator
    {
        #region Properties
        private const string GROUP_NAME_GENERAL = "1. General";
        private const string GROUP_NAME_POC_SMA = "2. Point Of Control SMA";

        #region General Properties

        [NinjaScriptProperty]
        [Display(Name = "Version", Description = "Point Of Control SMA Version", Order = 0, GroupName = GROUP_NAME_GENERAL)]
        [ReadOnly(true)]
        public string Version => "1.0.0";

        #endregion

        #region POC SMA

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Period", Description = "Period for the moving average calculation.", GroupName = GROUP_NAME_POC_SMA, Order = 0)]
        public int Period { get; set; }

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Volumetric Period", Description = "Period for volumetric data series.", GroupName = GROUP_NAME_POC_SMA, Order = 1)]
        public int VolumetricPeriod { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Volumetric Bars Type", Description = "Type of bars for volumetric data series.", GroupName = GROUP_NAME_POC_SMA, Order = 2)]
        public BarTypeOptions VolumetricBarsType { get; set; }

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Ticks Per Level", Description = "Number of ticks per volume profile level.", GroupName = GROUP_NAME_POC_SMA, Order = 3)]
        public int TicksPerLevel { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "PoC Moving Average", GroupName = "Plots", Order = 0)]
        public Series<double> PoCMovingAverage => Values[0];

        #endregion

        #endregion

        private VolumetricBarsType _volumetricBars;
        private int _volumetricBarsIndex = -1;
        private Series<double> _rawPoC;
        private SMA _smaSmooth;
        private double _lastPoC;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Calculates a Simple Moving Average of the Volume Point of Control.";
                Name = "_PointOfControlMovingAverage";
                Calculate = Calculate.OnEachTick;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                PaintPriceMarkers = true;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                Period = 8;
                VolumetricPeriod = 5;
                VolumetricBarsType = BarTypeOptions.Minute;
                TicksPerLevel = 5;

                AddPlot(Brushes.DodgerBlue, "PoC Moving Average");
            }
            else if (State == State.Configure)
            {
                BarsPeriodType barsPeriodType = VolumetricBarsType switch
                {
                    BarTypeOptions.Minute => BarsPeriodType.Minute,
                    BarTypeOptions.Range => BarsPeriodType.Range,
                    BarTypeOptions.Second => BarsPeriodType.Second,
                    BarTypeOptions.Tick => BarsPeriodType.Tick,
                    BarTypeOptions.Volume => BarsPeriodType.Volume,
                    _ => BarsPeriodType.Minute
                };

                AddVolumetric(Instrument.FullName, barsPeriodType, VolumetricPeriod, VolumetricDeltaType.BidAsk, TicksPerLevel);
            }
            else if (State == State.DataLoaded)
            {
                _volumetricBarsIndex = 1;
                _volumetricBars = BarsArray[_volumetricBarsIndex].BarsType as VolumetricBarsType;
                _rawPoC = new Series<double>(this);
                _smaSmooth = SMA(_rawPoC, Period);
            }
        }

        protected override void OnBarUpdate()
        {
            if (BarsInProgress == _volumetricBarsIndex)
            {
                if (!IsFirstTickOfBar || _volumetricBars == null || CurrentBars[_volumetricBarsIndex] < 1)
                    return;

                var vols = _volumetricBars.Volumes[CurrentBars[_volumetricBarsIndex] - 1];
                if (vols != null)
                {
                    vols.GetMaximumVolume(null, out _lastPoC);
                }
            }
            else if (BarsInProgress == 0)
            {
                if (CurrentBars[0] < 1)
                    return;

                _rawPoC[0] = _lastPoC;
                PoCMovingAverage[0] = _smaSmooth[0];
            }
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private PointOfControlMovingAverage[] cachePointOfControlMovingAverage;
        public PointOfControlMovingAverage PointOfControlMovingAverage(int period, int volumetricPeriod, BarTypeOptions volumetricBarsType, int ticksPerLevel)
        {
            return PointOfControlMovingAverage(Input, period, volumetricPeriod, volumetricBarsType, ticksPerLevel);
        }

        public PointOfControlMovingAverage PointOfControlMovingAverage(ISeries<double> input, int period, int volumetricPeriod, BarTypeOptions volumetricBarsType, int ticksPerLevel)
        {
            if (cachePointOfControlMovingAverage != null)
                for (int idx = 0; idx < cachePointOfControlMovingAverage.Length; idx++)
                    if (cachePointOfControlMovingAverage[idx] != null && cachePointOfControlMovingAverage[idx].Period == period && cachePointOfControlMovingAverage[idx].VolumetricPeriod == volumetricPeriod && cachePointOfControlMovingAverage[idx].VolumetricBarsType == volumetricBarsType && cachePointOfControlMovingAverage[idx].TicksPerLevel == ticksPerLevel && cachePointOfControlMovingAverage[idx].EqualsInput(input))
                        return cachePointOfControlMovingAverage[idx];
            return CacheIndicator<PointOfControlMovingAverage>(new PointOfControlMovingAverage() { Period = period, VolumetricPeriod = volumetricPeriod, VolumetricBarsType = volumetricBarsType, TicksPerLevel = ticksPerLevel }, input, ref cachePointOfControlMovingAverage);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.PointOfControlMovingAverage PointOfControlMovingAverage(int period, int volumetricPeriod, BarTypeOptions volumetricBarsType, int ticksPerLevel)
        {
            return indicator.PointOfControlMovingAverage(Input, period, volumetricPeriod, volumetricBarsType, ticksPerLevel);
        }

        public Indicators.PointOfControlMovingAverage PointOfControlMovingAverage(ISeries<double> input, int period, int volumetricPeriod, BarTypeOptions volumetricBarsType, int ticksPerLevel)
        {
            return indicator.PointOfControlMovingAverage(input, period, volumetricPeriod, volumetricBarsType, ticksPerLevel);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.PointOfControlMovingAverage PointOfControlMovingAverage(int period, int volumetricPeriod, BarTypeOptions volumetricBarsType, int ticksPerLevel)
        {
            return indicator.PointOfControlMovingAverage(Input, period, volumetricPeriod, volumetricBarsType, ticksPerLevel);
        }

        public Indicators.PointOfControlMovingAverage PointOfControlMovingAverage(ISeries<double> input, int period, int volumetricPeriod, BarTypeOptions volumetricBarsType, int ticksPerLevel)
        {
            return indicator.PointOfControlMovingAverage(input, period, volumetricPeriod, volumetricBarsType, ticksPerLevel);
        }
    }
}

#endregion
