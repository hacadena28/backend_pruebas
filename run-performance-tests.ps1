# GESTOR DE RENDIMIENTO - VERSIÓN LIMPIA (C:\Repos)
$basePath = $PSScriptRoot
$jmeterDir = Join-Path $basePath "jmeter"
$resultsFile = Join-Path $jmeterDir "results.jtl"
$reportDir = Join-Path $jmeterDir "report"

Write-Host "`n====================================================" -ForegroundColor Cyan
Write-Host "   GESTOR DE RENDIMIENTO - ENTORNO LOCAL LIMPIO    " -ForegroundColor Cyan
Write-Host "====================================================`n" -ForegroundColor Cyan

# 1. Limpieza
Write-Host "[1/3] Limpiando archivos previos..." -ForegroundColor Gray
if (Test-Path $resultsFile) { Remove-Item $resultsFile -Force }
if (Test-Path $reportDir) { Remove-Item $reportDir -Recurse -Force }

# 2. Ejecución
Write-Host "[2/3] Ejecutando prueba de carga (60 seg)..." -ForegroundColor Green
$runArgs = @(
    "run", "--rm",
    "-v", "${jmeterDir}:/jmeter",
    "-w", "/jmeter",
    "justb4/jmeter",
    "-n", "-t", "performance_test.jmx",
    "-l", "results.jtl",
    "-Jhost=host.docker.internal",
    "-Jport=5014",
    "-Jprotocol=http"
)
& docker $runArgs

# 3. Generación del Reporte
Write-Host "`n[3/3] Generando reporte visual..." -ForegroundColor Yellow
if (Test-Path $resultsFile) {
    $reportArgs = @(
        "run", "--rm",
        "-v", "${jmeterDir}:/jmeter",
        "-w", "/jmeter",
        "justb4/jmeter",
        "-g", "results.jtl",
        "-o", "report"
    )
    & docker $reportArgs

    if (Test-Path (Join-Path $reportDir "index.html")) {
        Write-Host "`n>>> PRUEBA COMPLETADA CON ÉXITO <<<`n" -ForegroundColor Green
        Start-Process (Join-Path $reportDir "index.html")
    }
} else {
    Write-Host "`n[!] ERROR: No se encontró results.jtl. Verifica los logs de Docker." -ForegroundColor Red
}
