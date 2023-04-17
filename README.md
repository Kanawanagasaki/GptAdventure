# GptAdventure

[![GptAdventure - [C#] 3D Console Engine, Azure Cognitive Services, GPT-J](https://img.youtube.com/vi/ddGhdDfORus/0.jpg)](https://youtu.be/ddGhdDfORus)

This project demonstrates the use of:
1. 3D Console Engine
2. GPT-J Model
3. Azure Cognitive Services (Speech-to-text and text-to-speech) 

## 3D Console Engine

Works only with windows old console terminals: cmd, powershell

Will not work with Windows Terminal or with VSCode's terminal

_Fun fact: if i will not render image to console buffer i will have ~150fps, while with rendering: ~10fps_

## [GPT-J Model](https://huggingface.co/EleutherAI/gpt-j-6b)

In order for gpt-j model to world `python 3.8` must be installed on user's PC

You also will need to install a [Hugging Face Hub](https://huggingface.co/docs/huggingface_hub/quick-start)

`python` script depends on `pytorch` and `transformers`
```
pip install pytorch --user
pip install transformers --user
```

### Model requirements:

```
1. Free Dick Space: 12GM
2. CPU RAM: 12GB
3. GPU VRAM: 16GM
```
## [Azure Cognitive Services](https://azure.microsoft.com/en-us/products/cognitive-services/#overview)

1. You need to sign up to [Microsoft Azure](https://azure.microsoft.com/en-us/). Note that you will need to enter your bank card information to register. However, Azure Speech Service will be free for you if you synthesize less than 500 000 characters per month
2. Go to [Azure Portal](https://portal.azure.com)
3. Select `Create a resource`
4. Select `AI + Machine Learning` > `Speech` > `Create`
5. Create your new resource, it might take few minutes to create resource
6. Create `azure.settings.config` file in project root directory with content:
```
APPNAME=<resource name>
KEY=<api key>
REGION=<resource region>
TTS_ENDPOINT=https://<resource region>.tts.speech.microsoft.com/cognitiveservices/v1
VOICES_ENDPOINT=https://<resource region>.tts.speech.microsoft.com/cognitiveservices/voices/list
RECOGNITION_ENDPOINT=https://<resource region>.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1
```
