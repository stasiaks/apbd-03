# Komentarze

Pozwoliłem sobie na autoformatowanie plików których "nie wolno modyfikować", zakładam, że pozostaje to w duchu zadania, jako że treść pozostaje bez zmian.
Nie chciałem specjalnie pod to wymaganie robić wyjątków w pipeline.

Przeniosłem także kod z `zadanie` do `src`, dla wygody.

## Email

Warunki w kodzie są fałszywe, ponieważ adres email nie musi zawierać kropki (np. używając adresu IPv6 jako części domenowej)
`example@[IPv6:292e:9bf5:0d8a:4174:56c5:6024:3a7f:32fd]`

Oczywiście zachowałem w kodzie jej wymóg.

## HasCreditLimit

Nie mam pojęcia co to miało znaczyć. Planowałem np. zmienić `CreditLimit` na nullable, i całkowicie to property usunąć, ale jest ono osobnym warunkiem, tylko dziwnie nazwanym.

Zmieniłem nazwę na `IsExemptFromCreditLimitMinimum`, ponieważ tyle mogłem się domyślić z warunków.