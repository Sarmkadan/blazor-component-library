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
            clickHandler: null,
            renderId: 0,
            disposed: false,
            geometryCache: null,
            abortController: null
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
        try {
            chartWrapper.element['setSeriesVisibility'](seriesIndex, newVisibility);
        } catch (error) {
            console.warn('Failed to invoke SetSeriesVisibility:', error);
        }
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

// Render or update the chart via JavaScript
window.blazorChartInterop.renderChart = async function (
    element,
    chartType,
    title,
    labels,
    colors,
    options,
    annotations,
    valueFormatter,
    geometryCache,
    dataHash,
    forceRefresh,
    dotNetObjectReference
) {
    try {
        // Check if this element already has chart interop initialized
        if (!chartInstances.has(element.id)) {
            // Initialize the chart wrapper
            const chartWrapper = {
                element: element,
                seriesVisibility: new Map(),
                clickHandler: null,
                renderId: 0,
                disposed: false,
                geometryCache: null,
                abortController: new AbortController()
            };
            chartInstances.set(element.id, chartWrapper);
        }

        const chartWrapper = chartInstances.get(element.id);
        if (!chartWrapper || chartWrapper.disposed) {
            return;
        }

        // Increment render ID to track if this render is still valid
        chartWrapper.renderId++;
        const currentRenderId = chartWrapper.renderId;

        // Store abort controller for this render
        const abortController = new AbortController();
        chartWrapper.abortController = abortController;

        // If forceRefresh is true, clear any cached geometry
        if (forceRefresh && chartWrapper.geometryCache) {
            chartWrapper.geometryCache = null;
        }

        // Store the current geometry cache in the wrapper for potential reuse
        if (!chartWrapper.geometryCache) {
            chartWrapper.geometryCache = new Map();
        }

        // Merge the provided geometry cache with our stored cache
        if (geometryCache) {
            for (const [key, value] of Object.entries(geometryCache)) {
                chartWrapper.geometryCache.set(key, value);
            }
        }

        // For Chart.js integration - this would be implemented based on the actual charting library
        // This is a placeholder that demonstrates the pattern
        if (typeof Chart !== 'undefined' && element) {
            // Check if we should still proceed with this render (not superseded by a newer one)
            if (chartWrapper.renderId !== currentRenderId) {
                abortController.abort();
                return;
            }

            // Get or create the chart instance
            let chart = Chart.getChart(element.id);

            if (!chart) {
                // Create a new chart if one doesn't exist
                const canvas = document.createElement('canvas');
                canvas.id = element.id + '-canvas';
                element.appendChild(canvas);

                // This would be replaced with actual chart configuration based on parameters
                const ctx = canvas.getContext('2d');
                if (ctx) {
                    chart = new Chart(ctx, {
                        type: chartType.toLowerCase(),
                        data: {
                            labels: labels || [],
                            datasets: []
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                legend: {
                                    onClick: function(e, legendItem, legend) {
                                        const index = legendItem.datasetIndex;
                                        if (index !== undefined && dotNetObjectReference) {
                                            try {
                                                dotNetObjectReference.invokeMethodAsync('SetSeriesVisibility', index, !(chart.data.datasets[index]?.hidden ?? false));
                                            } catch (error) {
                                                console.warn('Failed to invoke SetSeriesVisibility:', error);
                                            }
                                        }
                                        // Call original handler to maintain default behavior
                                        const originalOnClick = Chart.defaults.plugins.legend.onClick;
                                        if (originalOnClick) {
                                            originalOnClick(e, legendItem, legend);
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
            } else {
                // Update existing chart
                if (chart.data.labels !== labels) {
                    chart.data.labels = labels || [];
                }

                // Update datasets based on colors and data
                // This is simplified - actual implementation would need to handle the data properly
                if (colors && colors.length > 0) {
                    while (chart.data.datasets.length > colors.length) {
                        chart.data.datasets.pop();
                    }
                    while (chart.data.datasets.length < colors.length) {
                        chart.data.datasets.push({
                            label: 'Series ' + chart.data.datasets.length,
                            data: [],
                            backgroundColor: colors[chart.data.datasets.length % colors.length],
                            borderColor: colors[chart.data.datasets.length % colors.length],
                            borderWidth: 1,
                            hidden: false
                        });
                    }
                }

                chart.update();
            }
        }

        // Handle annotations if provided
        if (annotations && annotations.length > 0 && element) {
            // In a real implementation, this would add annotation elements to the chart container
            // For now, we just ensure the annotations are processed
            console.debug('Rendering', annotations.length, 'annotations');
        }

    } catch (error) {
        // Check if this is a disposal-related error that we can safely ignore
        if (error instanceof DOMException && error.name === 'AbortError') {
            // Component is being disposed, ignore this error
            console.debug('Chart render aborted during disposal');
        } else if (error instanceof ReferenceError && error.message.includes('disposed')) {
            // DotNetObjectReference was disposed
            console.debug('Chart render aborted due to disposed DotNetObjectReference');
        } else {
            console.error('Failed to render chart:', error);
        }
    }
};

// Clean up when component is disposed
window.blazorChartInterop.dispose = function (elementId) {
    const chartWrapper = chartInstances.get(elementId);
    if (chartWrapper) {
        chartWrapper.disposed = true;

        // Abort any pending render operations
        if (chartWrapper.abortController) {
            chartWrapper.abortController.abort();
        }

        // Remove event listeners
        if (chartWrapper.clickHandler) {
            chartWrapper.clickHandler();
        }

        // Try to clean up Chart.js instance if it exists
        try {
            const chart = Chart.getChart(elementId);
            if (chart) {
                chart.destroy();
            }
        } catch (error) {
            // Ignore cleanup errors
        }

        chartInstances.delete(elementId);
    }
};