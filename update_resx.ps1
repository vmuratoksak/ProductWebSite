[CmdletBinding()]
param ()

$baseDir = "c:\Users\vmura\source\repos\ProductWebSite\SatisSitesi\Resources"

$translations = @{
    "tr" = @{
        "Newest" = "En Yeni";
        "Oldest" = "En Eski";
        "Price_Asc" = "Fiyat (Artan)";
        "Price_Desc" = "Fiyat (Azalan)";
        "TotalDesc" = "Tutar (Azalan)";
        "TotalAsc" = "Tutar (Artan)"
    };
    "en" = @{
        "Newest" = "Newest";
        "Oldest" = "Oldest";
        "Price_Asc" = "Price (Low to High)";
        "Price_Desc" = "Price (High to Low)";
        "TotalDesc" = "Total (High to Low)";
        "TotalAsc" = "Total (Low to High)"
    };
    "de" = @{
        "Newest" = "Neueste";
        "Oldest" = "Älteste";
        "Price_Asc" = "Preis aufsteigend";
        "Price_Desc" = "Preis absteigend";
        "TotalDesc" = "Gesamt absteigend";
        "TotalAsc" = "Gesamt aufsteigend"
    };
    "fr" = @{
        "Newest" = "Le plus récent";
        "Oldest" = "Le plus ancien";
        "Price_Asc" = "Prix croissant";
        "Price_Desc" = "Prix décroissant";
        "TotalDesc" = "Total décroissant";
        "TotalAsc" = "Total croissant"
    };
    "ar" = @{
        "Newest" = "الأحدث";
        "Oldest" = "الأقدم";
        "Price_Asc" = "السعر (تصاعدي)";
        "Price_Desc" = "السعر (تنازلي)";
        "TotalDesc" = "الإجمالي (تنازلي)";
        "TotalAsc" = "الإجمالي (تصاعدي)"
    }
}

$files = @{
    "SharedResource.resx" = "en";
    "SharedResource.tr.resx" = "tr";
    "SharedResource.en.resx" = "en";
    "SharedResource.de.resx" = "de";
    "SharedResource.fr.resx" = "fr";
    "SharedResource.ar.resx" = "ar"
}

foreach ($fileName in $files.Keys) {
    $filePath = Join-Path $baseDir $fileName
    $lang = $files[$fileName]
    
    if (Test-Path $filePath) {
        Write-Host "Updating $fileName with $lang translations"
        $xml = [xml](Get-Content $filePath -Encoding UTF8)
        $root = $xml.SelectSingleNode("/root")
        
        $langTranslates = $translations[$lang]
        
        foreach ($key in $langTranslates.Keys) {
            $existing = $root.SelectSingleNode("data[@name='$key']")
            if (-not $existing) {
                Write-Host "Adding $key to $fileName"
                $newNode = $xml.CreateElement("data")
                $newNode.SetAttribute("name", $key)
                $newNode.SetAttribute("xml:space", "preserve")
                $valNode = $xml.CreateElement("value")
                $valNode.InnerText = $langTranslates[$key]
                $newNode.AppendChild($valNode) | Out-Null
                $root.AppendChild($newNode) | Out-Null
            } else {
                $existing.SelectSingleNode("value").InnerText = $langTranslates[$key]
            }
        }
        
        $xml.Save($filePath)
    }
}
Write-Host "Done!"
