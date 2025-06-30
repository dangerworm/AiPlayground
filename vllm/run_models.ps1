$basePort = 8000
$i = 0

# Read model names from models.txt
Get-Content "models.txt" | ForEach-Object {
    $model = $_.Trim()
    if ([string]::IsNullOrWhiteSpace($model)) { return }

    $port = $basePort + $i
    $safeName = $model -replace '[:/]', '--' -replace '[A-Z]', { $_.Value.ToLower() }

    # Default args
    $args = @(
        "--model", "$model",
        "--dtype", "float16",
        "--max-num-seqs", "1",
        "--swap-space", "4GiB"
    )

    # Custom model tweaks
    if ($model -like "*phi-3*") {
        $args = @(
            "--model", "$model",
            "--dtype", "float16",
            "--max-num-seqs", "2"
        )
    }
    elseif ($model -like "*llama-3*") {
        $args += "--max-model-len"
        $args += "1024"
    }
    elseif ($model -like "*falcon*") {
        $args[5] = "6GiB"  # Increase swap-space
        $args += "--max-model-len"
        $args += "1024"
    }

    # Run Docker
    docker run -d --runtime=nvidia --gpus all `
        --name "vllm-$safeName" `
        -v "D:/HuggingFace/.cache:/root/.cache/huggingface" `
        -e "HUGGING_FACE_HUB_TOKEN=$env:HUGGING_FACE_HUB_TOKEN" `
        -p "$port`:8000" `
        --ipc=host `
        vllm/vllm-openai:latest `
        $args

    Write-Output "✅ Started $model on http://localhost:$port"
    $i++
}
