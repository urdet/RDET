import { appName } from '../../i18n';

const lightLogo = new URL('../../../assets/ChatGPT Image 21 juin 2026, 17_45_31 (1).png', import.meta.url).href;
const darkLogo = new URL('../../../assets/ChatGPT Image 21 juin 2026, 17_45_32 (2).png', import.meta.url).href;

type BrandLogoProps = {
  variant?: 'mark' | 'full';
};

export function BrandLogo({ variant = 'mark' }: BrandLogoProps) {
  return (
    <span className={`brand-logo ${variant}`} aria-label={appName}>
      <img className="brand-logo-light" src={lightLogo} alt="" />
      <img className="brand-logo-dark" src={darkLogo} alt="" />
    </span>
  );
}
