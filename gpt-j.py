import transformers as tf
import base64
import torch
import sys
import os

model_cache_dir = os.path.join(
    os.path.dirname(os.path.realpath(__file__)), "model_cache"
)
if not os.path.exists(model_cache_dir):
    os.makedirs(model_cache_dir)

device = "cuda"
model = tf.GPTJForCausalLM.from_pretrained(
    "EleutherAI/gpt-j-6B",
    revision="float16",
    torch_dtype=torch.float16,
    low_cpu_mem_usage=True,
    cache_dir=model_cache_dir,
).to(device)
tokenizer = tf.AutoTokenizer.from_pretrained("EleutherAI/gpt-j-6B")

print("<waiting_for_prompt>", flush=True)

for line in sys.stdin:
    try:
        prompt = base64.b64decode(line).decode('utf-8')
        input_ids = tokenizer(prompt, return_tensors="pt").input_ids.to(device)
        gen_tokens = model.generate(
            input_ids,
            do_sample=True,
            temperature=0.9,
            max_length=input_ids.size(1) + 64,
            pad_token_id=tokenizer.eos_token_id,
        )
        result = tokenizer.batch_decode(gen_tokens)[0]
        b64 = base64.b64encode(result.encode('utf-8')).decode("ascii")
        print("<gpt_result>" + b64, flush=True)
        print("<waiting_for_prompt>", flush=True)
    except Exception as e:
        sys.stderr.write("Exception: " + str(e) + "\n")
        sys.stderr.flush()
