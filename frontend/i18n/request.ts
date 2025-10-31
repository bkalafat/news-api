import { getRequestConfig } from "next-intl/server";

export default getRequestConfig(async () => {
  // Currently only Turkish is supported
  const locale = "tr";

  return {
    locale,
    messages: (await import(`../messages/${locale}.json`)).default,
  };
});
