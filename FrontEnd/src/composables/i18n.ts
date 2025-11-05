import { createI18n } from 'vue-i18n'
import en from '../locales/en.json'
import pt from '../locales/pt.json'
import zh from '../locales/zh.json'
import ca from '../locales/ca.json'
import bg from '../locales/bg.json'
import cs from '../locales/cs.json'
import da from '../locales/da.json'
import de from '../locales/de.json'
import el from '../locales/el.json'
import es from '../locales/es.json'
import et from '../locales/et.json'
import fi from '../locales/fi.json'
import fr from '../locales/fr.json'
import hr from '../locales/hr.json'
import hu from '../locales/hu.json'
import is from '../locales/is.json'
import it from '../locales/it.json'
import lt from '../locales/lt.json'
import lv from '../locales/lv.json'
import mt from '../locales/mt.json'
import nl from '../locales/nl.json'
import no from '../locales/no.json'
import pl from '../locales/pl.json'
import ro from '../locales/ro.json'
import ru from '../locales/ru.json'
import sk from '../locales/sk.json'
import sl from '../locales/sl.json'
import sv from '../locales/sv.json'
import uk from '../locales/uk.json'
import tr from '../locales/tr.json'
import ga from '../locales/ga.json'
import sq from '../locales/sq.json'
import mk from '../locales/mk.json'
import sr from '../locales/sr.json'
import bs from '../locales/bs.json'

const messages = { en, pt, zh, ca, bg, cs, da, de, el, es, et, fi, fr, hr, hu, is, it, lt, lv, mt, nl, no, pl, ro, ru, sk, sl, sv, uk, tr, ga, sq, mk, sr, bs };

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
