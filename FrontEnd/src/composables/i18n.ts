import { createI18n } from 'vue-i18n'
import en from '../locales/en.json'
import pt from '../locales/pt.json'
import zh from '../locales/zh.json'
import ca from '../locales/ca.json'

const messages = { en, pt, zh, ca }

let initialLocale = 'en'
if (typeof window !== 'undefined') {
  initialLocale = (localStorage.getItem('locale') as string) || (navigator.language ? navigator.language.split('-')[0] : 'en') || 'en'
}

const i18n = createI18n({
  legacy: false,
  locale: initialLocale,
  fallbackLocale: 'en',
  messages
})

export default i18n
