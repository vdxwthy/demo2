# Скрипт для полного удаления привязки к Git (включая родительские папки)

function Remove-GitArtifacts {
    param (
        [string]$Path
    )

    if (Test-Path -Path $Path) {
        Write-Host "Проверка папки: $Path" -ForegroundColor Cyan
        
        # Удаляем папку .git
        $gitDir = Join-Path -Path $Path -ChildPath ".git"
        if (Test-Path -Path $gitDir) {
            Write-Host "Найдена папка .git в $Path. Удаление..." -ForegroundColor Yellow
            try {
                Remove-Item -Path $gitDir -Recurse -Force -ErrorAction Stop
                Write-Host "Папка .git успешно удалена." -ForegroundColor Green
            }
            catch {
                Write-Host "Ошибка при удалении .git: $_" -ForegroundColor Red
            }
        }

        # Удаляем .gitattributes
        $gitAttr = Join-Path -Path $Path -ChildPath ".gitattributes"
        if (Test-Path -Path $gitAttr) {
            Write-Host "Найден файл .gitattributes в $Path. Удаление..." -ForegroundColor Yellow
            try {
                Remove-Item -Path $gitAttr -Force -ErrorAction Stop
                Write-Host "Файл .gitattributes успешно удален." -ForegroundColor Green
            }
            catch {
                Write-Host "Ошибка при удалении .gitattributes: $_" -ForegroundColor Red
            }
        }

        # Удаляем .gitignore
        $gitIgnore = Join-Path -Path $Path -ChildPath ".gitignore"
        if (Test-Path -Path $gitIgnore) {
            Write-Host "Найден файл .gitignore в $Path. Удаление..." -ForegroundColor Yellow
            try {
                Remove-Item -Path $gitIgnore -Force -ErrorAction Stop
                Write-Host "Файл .gitignore успешно удален." -ForegroundColor Green
            }
            catch {
                Write-Host "Ошибка при удалении .gitignore: $_" -ForegroundColor Red
            }
        }
    }
}

# Текущая папка
$currentDir = Get-Location
Remove-GitArtifacts -Path $currentDir

# Родительская папка
$parentDir = Split-Path -Parent $currentDir
Remove-GitArtifacts -Path $parentDir

# Папка еще выше (на всякий случай, если структура глубже)
$grandParentDir = Split-Path -Parent $parentDir
Remove-GitArtifacts -Path $grandParentDir

Write-Host "`nГотово! Перезапустите Visual Studio, чтобы изменения вступили в силу." -ForegroundColor Magenta