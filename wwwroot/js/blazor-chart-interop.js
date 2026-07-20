// Chart.js interop for Blazor Component Library
// Handles legend item click-to-toggle series visibility

// Global namespace for chart interop
window.blazorChartInterop = window.blazorChartInterop || {};

// Store chart instances by element ID
const chartInstances = new Map();

// Initialize chart interop for a specific chart element
window.blazorChartInterop.initialize = async function (element) {
  try {
    // Check if this element already has chart interop initialized
    if (chartInstances.has(element.id)) {
      return;
    }

    // Create a wrapper object for this chart
    const chartWrapper = {
      element: element,
      seriesVisibility: new Map(), // Track visibility state for each series
      clickHandler: null
    };

    chartInstances.set(element.id, chartWrapper);

    // Add click event listener to legend items
    // This assumes Chart.js is being used, which renders legend items with specific classes
    const legendItems = element.querySelectorAll('.chart-legend-item');
    if (legendItems && legendItems.length > 0) {
      legendItems.forEach((item, index) => {
        item.style.cursor = 'pointer';
        item.addEventListener('click', () => {
          toggleSeriesVisibility(element.id, index);
        });
      });
    }

    // For Chart.js, we need to handle the legend click event differently
    // Chart.js provides a built-in legend click handler that we can override
    if (typeof Chart !== 'undefined') {
      // Override Chart.js legend onClick handler to enable legend item click-to-toggle
      if (Chart.defaults && Chart.defaults.plugins && Chart.defaults.plugins.legend) {
        // Store original handler
        const originalOnClick = Chart.defaults.plugins.legend.onClick;

        // Override with our custom handler
        Chart.defaults.plugins.legend.onClick = function(e, legendItem, legend) {
          const index = legendItem.datasetIndex;
          if (index !== undefined) {
            toggleSeriesVisibility(element.id, index);
          }
          // Call original handler to maintain default behavior
          if (originalOnClick) {
            originalOnClick(e, legendItem, legend);
          }
        };
      }
    }

  } catch (error) {
    console.error('Failed to initialize chart interop:', error);
  }
};

// Toggle series visibility
function toggleSeriesVisibility(elementId, seriesIndex) {
  const chartWrapper = chartInstances.get(elementId);
  if (!chartWrapper) {
    return;
  }

  // Toggle visibility state
  const currentVisibility = chartWrapper.seriesVisibility.get(seriesIndex) ?? true;
  const newVisibility = !currentVisibility;

  chartWrapper.seriesVisibility.set(seriesIndex, newVisibility);

  // Update chart visibility
  updateChartVisibility(elementId, seriesIndex, newVisibility);

  // Call Blazor method to update state
  if (chartWrapper.element && chartWrapper.element['setSeriesVisibility']) {
    chartWrapper.element['setSeriesVisibility'](seriesIndex, newVisibility);
  }
}

// Update chart visibility using Chart.js API
function updateChartVisibility(elementId, seriesIndex, visible) {
  const chartWrapper = chartInstances.get(elementId);
  if (!chartWrapper) {
    return;
  }

  try {
    // If Chart.js is available, use its API
    if (typeof Chart !== 'undefined') {
      const chart = Chart.getChart(elementId);
      if (chart) {
        // Update dataset visibility
        chart.data.datasets[seriesIndex].hidden = !visible;
        chart.update();
      }
    }
    // For other charting libraries, we would implement library-specific logic here

  } catch (error) {
    console.error('Failed to update chart visibility:', error);
  }
}

// Clean up chart interop when element is removed
window.blazorChartInterop.dispose = function (elementId) {
  const chartWrapper = chartInstances.get(elementId);
  if (chartWrapper) {
    // Remove event listeners
    if (chartWrapper.clickHandler) {
      chartWrapper.clickHandler();
    }
    chartInstances.delete(elementId);
  }
};