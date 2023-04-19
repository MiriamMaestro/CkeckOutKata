using FluentAssertions;

namespace Checkout
{
    public class CheckOutKata
    {
        private readonly CheckOut _checkOut;

        public CheckOutKata() => _checkOut = new CheckOut();

        [Theory]
        [InlineData("A", 50)]
        [InlineData("B", 30)]
        [InlineData("C", 20)]
        [InlineData("D", 15)]
        public void WhenScanningAnIndividualProduct(string productName, int amount)
        {
            _checkOut.Scan(new Product(productName));

            _checkOut.GetTotalPrice().Should().BeEquivalentTo(new TotalPrice(amount));
        }

        [Fact]
        public void WhenScanningOneOfEachProductThePriceIsCorrect()
        {
            _checkOut.Scan(new Product("A"));
            _checkOut.Scan(new Product("B"));
            _checkOut.Scan(new Product("C"));
            _checkOut.Scan(new Product("D"));

            _checkOut.GetTotalPrice().Should().BeEquivalentTo(new TotalPrice(115));
        }

        [Fact]
        public void WhenScanningThreeATheDiscountPriceWithDiscount()
        {
            _checkOut.Scan(new Product("A"));
            _checkOut.Scan(new Product("A"));
            _checkOut.Scan(new Product("A"));

            _checkOut.GetTotalPrice().Should().Be(new TotalPrice(130));
        }

        [Fact]
        public void WhenScanningTwoBTheDiscountPriceWithDiscount()
        {
            _checkOut.Scan(new Product("B"));
            _checkOut.Scan(new Product("B"));

            _checkOut.GetTotalPrice().Should().Be(new TotalPrice(45));
        }

        [Fact]
        public void WhenScanningTwoBAndThreeATheDiscountPriceWithDiscount()
        {
            _checkOut.Scan(new Product("A"));
            _checkOut.Scan(new Product("A"));
            _checkOut.Scan(new Product("A"));
            _checkOut.Scan(new Product("B"));
            _checkOut.Scan(new Product("B"));

            _checkOut.GetTotalPrice().Should().Be(new TotalPrice(175));
        }
    }

    public class CheckOut
    {
        private TotalPrice _totalPrice = new(0);
        private readonly ProductCheckout _productCheckout = new();

        private readonly Dictionary<Product, TotalPrice> _priceList = new()
        {
            { new Product("A"), new TotalPrice(50) },
            { new Product("B"), new TotalPrice(30) },
            { new Product("C"), new TotalPrice(20) },
            { new Product("D"), new TotalPrice(15) }
        };

        private readonly Dictionary<Product, TotalPrice> _priceListWithDiscount = new()
        {
            { new Product("A"), new TotalPrice(30) },
            { new Product("B"), new TotalPrice(15) }
        };

        public void Scan(Product product)
        {
            _productCheckout.Add(product);

            _totalPrice = _totalPrice.Add(DiscountAvailable(product)
                ? new TotalPrice(GetDiscountedPrice(product).Price)
                : new TotalPrice(GetProductPrice(product).Price));
        }

        public TotalPrice GetProductPrice(Product product)
        {
            return _priceList[product];
        }

        public TotalPrice GetTotalPrice()
        {
            return new TotalPrice(_totalPrice.Price);

        }

        public TotalPrice GetDiscountedPrice(Product product)
        {
            return _priceListWithDiscount[product];
        }

        private bool DiscountAvailable(Product product)
        {
            if (product.Equals(new Product("A"))
                && _productCheckout.ProductCheckList
                    .Count(x => x.Equals(new Product("A"))) % 3 == 0)
            {
                return true;
            }

            return product.Equals(new Product("B"))
                   && _productCheckout.ProductCheckList
                       .Count(x => x.Equals(new Product("B"))) % 2 == 0;
        }
    }

    public class ProductCheckout
    {
        public ProductCheckout()
        {
            ProductCheckList = new List<Product>();
        }

        public List<Product> ProductCheckList { get; set; }

        public List<Product> Add(Product product)
        {
            ProductCheckList.Add(product);
            return ProductCheckList;
        }
    }


    public record Product(string Name)
    {

    }

    public record TotalPrice(int Price)
    {
        public TotalPrice Add(TotalPrice totalPrice)
        {
            return new TotalPrice(totalPrice.Price + Price);
        }
    }
}