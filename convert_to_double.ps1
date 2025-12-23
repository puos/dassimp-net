# convert_to_double.ps1
$files = Get-ChildItem -Path "D:\work\dassimp-net\AssimpNet" -Filter "*.cs" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file. FullName -Raw
    
    # public float -> public double
    $content = $content -replace 'public float ', 'public double '
    
    # private float -> private double
    $content = $content -replace 'private float ', 'private double '
    
    # (float) 캐스트 제거
    $content = $content -replace '\(float\)', '(double)'
    
    Set-Content $file.FullName -Value $content
}

Write-Host "변환 완료!"