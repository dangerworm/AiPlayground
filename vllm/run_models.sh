while read model; do
  port=$((base_port + i))
  safe_name=$(echo "$model" | tr '/:' '--' | tr '[:upper:]' '[:lower:]')

  args=("--model" "$model" "--dtype" "float16" "--max-num-seqs" "1" "--swap-space" "4GiB")

  if [[ "$model" == *"phi-3"* ]]; then
    args=("--model" "$model" "--dtype" "float16" "--max-num-seqs" "2")
  fi

  docker run -d --runtime=nvidia --gpus all \
    --name "vllm-$safe_name" \
    -v /mnt/d/huggingface_cache:/root/.cache/huggingface \
    -e HUGGING_FACE_HUB_TOKEN=$HUGGING_FACE_HUB_TOKEN \
    -p ${port}:8000 \
    --ipc=host \
    vllm/vllm-openai:latest \
    "${args[@]}"

  echo "Started $model on port $port"
  ((i++))
done < models.txt
