<template>
  <div class="chatbox-container">
    <div class="container">
      <h1>AI Chat Bot</h1>
      <div ref="messageBox" class="messageBox mt-8">
        <template v-for="(message, index) in messages" :key="index">
          <div :class="message.role == 'user'? 'messageFromUser' : 'messageFromChatGpt'">
            <div
              :class="(message.role == 'user' ||  message.content.trim().length !== 0)? 'userMessageWrapper' : 'chatGptMessageWrapper'">
              <div :class="message.role == 'user'  ? 'userMessageContent' : 'chatGptMessageContent'">
                <div :class=" message.content.trim().length === 0 ? 'dot-flashing' : ''">
                  {{ message.content }}
                </div>
              </div>
            </div>
            <img v-if="message.role == 'assistant'" :src='require(`../assets/bot.png`)' width="40" height="40">
          </div>
        </template>
      </div>
      <div class="inputContainer">
        <input v-model="currentMessage" type="text" class="messageInput" :disabled="processing"
          placeholder="Please enter your question" @keyup.enter="sendMessage(currentMessage)" />
        <button :disabled="processing" @click="sendMessage(currentMessage)" class="askButton">
          <i class="pi pi-send" style="font-size: 1rem"></i>
        </button>
      </div>
    </div>
  </div>
</template>


<script>
import axios from 'axios';
import 'primeicons/primeicons.css'


export default {
  name: 'ChatBox',
  data() {
    return {
      currentMessage: '',
      system_prompt: {
        "role": "system",
        "content": "you are a helpful assiatant."
      },
      processing: false,
      messages: [],
    };
  },
  methods: {
    async sendMessage(currentMessage) {
      if (currentMessage.trim().length === 0) {
        return
      }
      this.processing = true
      //update messages
      this.messages.push({
        role: 'user',
        content: currentMessage,
      });

      var submitted_messages = []
      submitted_messages.push(this.system_prompt) //Fristly, insert the system prompt 
      //only remember recently 7 content
      let remember_num = 7
      if (this.messages.length > remember_num) {
        for (let i = 0; i < remember_num; ++i) {
          submitted_messages[i + 1] = this.messages[this.messages.length - remember_num + i];
        }
      } else {
        for (let i = 0; i < this.messages.length; ++i) {
          submitted_messages[i + 1] = this.messages[i]
        }
      }

      this.messages.push({
        role: 'assistant',
        content: '',
      });
      await axios
        .post('https://localhost:7152/api/OpenAIchat', submitted_messages
        )
        .then((response) => {
          this.messages[this.messages.length - 1].content = response.data // Access the 'data' property of the response object
        }).catch(function (error) {
          console.log(error.toJSON());
        });
      this.currentMessage = ''
      this.processing = false
    }
  },
  updated: function () {
    //after render all message, scroll down 
    const messageBox = this.$refs.messageBox;
    messageBox.scrollTop = messageBox.scrollHeight;
  },
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;500&display=swap');

.chatbox-container {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 1000;
}

.container {
  width: 360px;
  height: 600px;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  font-family: 'Roboto', sans-serif;
}

h1 {
  font-size: 24px;
  font-weight: 500;
  text-align: center;
  color: #222;
  padding: 16px;
  margin: 0;
  background-color: #f7f7f7;
  border-bottom: 1px solid #e7e7e7;
}

/**
 * ==============================================
 * Dot Flashing
 * ==============================================
 */
.dot-flashing {
  position: relative;
  left: 10px;
  width: 10px;
  height: 10px;
  border-radius: 5px;
  background-color: #000000;
  color: #000000;
  animation: dot-flashing 1s infinite linear alternate;
  animation-delay: 0.5s;
}

.dot-flashing::before,
.dot-flashing::after {
  content: "";
  display: inline-block;
  position: absolute;
  top: 0;
}

.dot-flashing::before {
  left: -15px;
  width: 10px;
  height: 10px;
  border-radius: 5px;
  background-color: #000000;
  color: #000000;
  animation: dot-flashing 1s infinite alternate;
  animation-delay: 0s;
}

.dot-flashing::after {
  left: 15px;
  width: 10px;
  height: 10px;
  border-radius: 5px;
  background-color: #000000;
  color: #000000;
  animation: dot-flashing 1s infinite alternate;
  animation-delay: 1s;
}

@keyframes dot-flashing {
  0% {
    background-color: #000000;
  }

  50%,
  100% {
    background-color: rgba(26, 26, 28, 0.2);
  }
}

.messageBox {
  padding: 16px;
  flex-grow: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.messageFromUser,
.messageFromChatGpt {
  display: flex;
}


.messageBox {
  max-height: 450px;
  overflow-y: auto;
  padding: 0 16px;
  border-top: 1px solid #f0f0f0;
  border-bottom: 1px solid #f0f0f0;
  flex-grow: 1;
}

.messageFromUser,
.messageFromChatGpt {
  display: flex;
  margin-bottom: 2px;
}

.userMessageWrapper,
.chatGptMessageWrapper {
  display: flex;
  min-width: 60px;
  flex-direction: column;
}

.userMessageWrapper {
  align-self: flex-end;
}

.chatGptMessageWrapper {
  align-self: flex-start;
}

.userMessageContent,
.chatGptMessageContent {
  max-width: 85%;
  padding: 8px 12px;
  border-radius: 18px;
  margin-bottom: 2px;
  font-size: 14px;
  line-height: 1.4;
}

.userMessageContent {
  background-color: #1877F2;
  color: white;
  border-top-left-radius: 0;
}

.chatGptMessageContent {
  background-color: #EDEDED;
  color: #222;
  border-top-right-radius: 0;
}

.userMessageTimestamp,
.chatGptMessageTimestamp {
  font-size: 10px;
  color: #999;
  margin-top: 2px;
}

.userMessageTimestamp {
  align-self: flex-end;
}

.chatGptMessageTimestamp {
  align-self: flex-start;
}

.inputContainer {
  display: flex;
  align-items: center;
  padding: 8px;
  background-color: #f0f0f0;
}

.messageInput {
  flex-grow: 1;
  border: none;
  outline: none;
  padding: 12px;
  font-size: 16px;
  background-color: white;
  border-radius: 24px;
  margin-right: 8px;
}

.askButton {
  background-color: #1877F2;
  color: white;
  font-size: 16px;
  padding: 8px 16px;
  border: none;
  outline: none;
  cursor: pointer;
  border-radius: 24px;
  transition: background-color 0.3s ease-in-out;
}

.askButton:hover {
  background-color: #145CB3;
}

@media (max-width: 480px) {
  .container {
    width: 100%;
    max-width: none;
    border-radius: 0;
  }
}

.chatbox-container {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 1000;
}


.messageBox {
  padding: 20px;
  flex-grow: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.messageFromUser,
.messageFromChatGpt {
  display: flex;
}
</style>