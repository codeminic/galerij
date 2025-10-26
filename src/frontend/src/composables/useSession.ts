import { ref } from 'vue'

const SESSION_ID_KEY = 'gallery-session-id'

export function useSession() {
  const sessionId = ref<string>(getOrCreateSessionId())

  function getOrCreateSessionId(): string {
    let id = localStorage.getItem(SESSION_ID_KEY)
    if (!id) {
      id = generateUUID()
      localStorage.setItem(SESSION_ID_KEY, id)
    }
    return id
  }

  function generateUUID(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      const r = (Math.random() * 16) | 0
      const v = c === 'x' ? r : (r & 0x3) | 0x8
      return v.toString(16)
    })
  }

  function getSessionHeader(): { [key: string]: string } {
    return {
      'X-Session-Id': sessionId.value
    }
  }

  return {
    sessionId,
    getSessionHeader
  }
}
