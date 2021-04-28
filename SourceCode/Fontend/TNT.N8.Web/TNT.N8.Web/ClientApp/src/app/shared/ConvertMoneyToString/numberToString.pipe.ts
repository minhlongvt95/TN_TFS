import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'numbertostring'
})
export class NumberToStringPipe implements PipeTransform {
    decimalString: string = '';
    transform(money: number, args?: any): any {
        const threeDigitsToLetter = (num) => {
            var letterOfnum = [' không ', ' một ', ' hai ', ' ba ', ' bốn ', ' năm ', ' sáu ', ' bảy ', ' tám ', ' chín '];
            let tram, chuc, donvi, _result = '';
            tram = Math.floor(num / 100);
            chuc = Math.floor(num % 100 / 10);
            donvi = num % 10;
            if (tram == 0 && chuc == 0 && donvi == 0) return '';
            if (tram != 0) {
                _result += letterOfnum[tram] + 'trăm ';
                if ((chuc == 0) && (donvi != 0)) _result += 'linh';
            }
            if ((chuc != 0) && (chuc != 1)) {
                _result += letterOfnum[chuc] + 'mươi ';
                if ((chuc == 0) && (donvi != 0)) _result = _result + 'linh';
            }
            if (chuc == 1) _result += 'mười';
            switch (donvi) {
                case 1:
                    if ((chuc != 0) && (chuc != 1)) {
                        _result += 'mốt';
                    }
                    else {
                        _result += letterOfnum[donvi];
                    }
                    break;
                case 5:
                    if (chuc == 0) {
                        _result += letterOfnum[donvi];
                    }
                    else {
                        _result += ' lăm';
                    }
                    break;
                default:
                    if (donvi != 0) {
                        _result += letterOfnum[donvi];
                    }
                    break;
            }
            return _result;
        };

        const handleMoney = (number) => {
            if (number && (number % 1 === 0)) {
                var count = 0, i = 0, num = number, _result = '';
                var tmp;
                var locate = new Array();
                var unitOfnum = ['', 'nghìn', 'triệu', 'tỷ', 'nghìn tỷ', 'triệu tỷ'];
                locate[5] = Math.floor(num / 1000000000000000);
                if (isNaN(locate[5]))
                    locate[5] = '0';
                num = num - parseFloat(locate[5].toString()) * 1000000000000000;
                locate[4] = Math.floor(num / 1000000000000);
                if (isNaN(locate[4]))
                    locate[4] = '0';
                num = num - parseFloat(locate[4].toString()) * 1000000000000;
                locate[3] = Math.floor(num / 1000000000);
                if (isNaN(locate[3]))
                    locate[3] = '0';
                num = num - parseFloat(locate[3].toString()) * 1000000000;
                locate[2] = Math.floor(num / 1000000);
                if (isNaN(locate[2]))
                    locate[2] = '0';
                locate[1] = Math.floor((num % 1000000) / 1000);
                if (isNaN(locate[1]))
                    locate[1] = '0';
                locate[0] = Math.floor(num % 1000);
                if (isNaN(locate[0]))
                    locate[0] = '0';
                if (locate[5] > 0) {
                    count = 5;
                }
                else if (locate[4] > 0) {
                    count = 4;
                }
                else if (locate[3] > 0) {
                    count = 3;
                }
                else if (locate[2] > 0) {
                    count = 2;
                }
                else if (locate[1] > 0) {
                    count = 1;
                }
                else {
                    count = 0;
                }
                for (i = count; i >= 0; i--) {
                    tmp = threeDigitsToLetter(locate[i]);
                    _result += ' ' + tmp.trim();
                    if (locate[i] > 0) _result += ' ' + unitOfnum[i];
                }
                _result = _result.substring(1, 2).toUpperCase() + _result.substring(2);
                if (_result !== '') {
                    return _result.trim() + ' đồng';
                } else {
                    return '';
                }
            }
            return number;
        };

        const handleDecimal = (decimalString: string) => {
            let letterOfnum = [' không ', ' một ', ' hai ', ' ba ', ' bốn ', ' năm ', ' sáu ', ' bảy ', ' tám ', ' chín '];
            let _resutl = '';
            let decimalLength: number = decimalString.length;

            //Loại các số không ở cuối phần thập phân
            this.filterTheLastNumberZero(decimalString);
            decimalString = this.decimalString;
            decimalLength = decimalString.length;

            for (let i = 0; i < decimalLength; i++) {
                let numString = '';
                if (i == 0) {
                    numString = decimalString.slice(0, 1);
                    if (decimalLength != 1) {
                        _resutl = letterOfnum[parseInt(numString)].trim();
                    } else {
                        _resutl = letterOfnum[parseInt(numString)].slice(1);
                    }
                }
                else if (i == decimalLength - 1) {
                    numString = decimalString.slice(i);
                    if (decimalLength != 2) {
                        _resutl += letterOfnum[parseInt(numString)].slice(1);
                    } else {
                        _resutl += letterOfnum[parseInt(numString)];
                    }
                }
                else {
                    numString = decimalString.slice(i,i + 1);
                    _resutl += letterOfnum[parseInt(numString)];
                }
            }

            return _resutl + 'đồng';
        };

        //Số chữ số lớn nhấn của phần thập phân
        let maximumFractionDigits: string = '';
        if (args) {
            maximumFractionDigits = args;
            //Làm tròn số chữ số phần thập phân
            money = Number((money).toLocaleString(
                        'en-US',
                        {minimumFractionDigits: 0, maximumFractionDigits: parseInt(maximumFractionDigits, 10)},
                    ).replace(/,/g,''));
        }
        if (money === undefined || money === null) {
            return '';
        }
        let moneyString: string = money.toString();
        if (moneyString.indexOf(".") != -1) 
        {
            let integerPartString: string = moneyString.slice(0, moneyString.indexOf("."));
            let integerPart: number = parseInt(integerPartString, 10);  //lấy số phần nguyên

            let decimalString: string = moneyString.slice(moneyString.indexOf(".") + 1);
            let decimal: number = parseInt(decimalString, 10);  //lấy số phần thập phân
            
            //Nếu chỉ có dấu '.' hoặc .00 thì bỏ qua phần thập phân 
            if (isNaN(decimal) || decimal == 0) 
            {
                return handleMoney(integerPart);
            }
            else 
            {
                let integerPartToString = handleMoney(integerPart).replace('đồng', '');    //chuyển phần nguyên thành chữ
                let decimalPartToString = handleDecimal(decimalString);   //chuyển phần thập phân thành chữ
                return integerPartToString + 'phẩy ' + decimalPartToString;
            }
        } 
        else 
        {
            let integerPart: number = parseInt(moneyString, 10);
            return handleMoney(integerPart);
        }
    }

    //Loại các số không ở cuối phần thập phân
    filterTheLastNumberZero(decimal: string): string {
        this.decimalString = decimal;
        var length = decimal.length;
        var lastNumber = decimal.slice(decimal.length - 1);
        if (lastNumber === "0") {
            this.decimalString = decimal.slice(0, length - 1);
            this.filterTheLastNumberZero(this.decimalString);
        } else {
            return this.decimalString;
        }
    };
}

