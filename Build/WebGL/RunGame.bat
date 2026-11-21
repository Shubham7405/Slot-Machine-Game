@echo off
powershell -NoExit -Command "$h = New-Object Net.HttpListener; $h.Prefixes.Add('http://localhost:8000/'); $h.Start(); Start-Process 'http://localhost:8000/'; while ($h.IsListening) { $c = $h.GetContext(); $p = $c.Request.Url.LocalPath.TrimStart('/'); if (!$p) { $p = 'index.html' }; if (Test-Path $p) { $b = [IO.File]::ReadAllBytes($p); if ($p -like '*.gz') { $c.Response.AddHeader('Content-Encoding','gzip') }; if ($p -like '*.br') { $c.Response.AddHeader('Content-Encoding','br') }; $c.Response.OutputStream.Write($b, 0, $b.Length) } else { $c.Response.StatusCode = 404 }; $c.Response.Close() }"
timeout /t 2 /nobreak >nul
start "" "http://localhost:8000"
pause

