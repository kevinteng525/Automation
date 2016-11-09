class ProductsController < ApplicationController
  before_action :set_product, only: [:show, :edit, :update, :destroy]

  # GET /products
  # GET /products.json
  def index
    @products = Product.get_all_force_products
  end

  # GET /products/1
  # GET /products/1.json
  def show
  end

  # GET /products/new
  def new
    @product = Product.new
  end

  # GET /products/1/edit
  def edit
      def @product.new_record?()
        false
    end
  end

  # POST /products
  # POST /products.json
  def create    
    respond_to do |format|
      if Product.create_force_product(product_params)
        format.html { redirect_to products_path, notice: 'Product was successfully created.' }
        format.json { render action: 'show', status: :created, location: @product }
      else
        format.html { render action: 'new' }
        format.json { render json: @product.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /products/1
  # PATCH/PUT /products/1.json
  def update
    respond_to do |format|
      if Product.update_force_product(params[:id], product_params)
        format.html { redirect_to @product, notice: 'Product was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @product.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /products/1
  # DELETE /products/1.json
  def destroy
    Product.delete_force_product(params[:id])
    respond_to do |format|
      format.html { redirect_to products_url }
      format.json { head :no_content }
    end
  end

  # GET /products/1/branches
  def branches
    @branches = Product.get_force_branches_of_product(params[:id],params[:type])
    respond_to do |format|
     format.json {render(:template => 'branches/index')}
    end
  end

  # GET /products/1/releases
  def releases
    @releases = Product.get_force_releases_of_product(params[:id],params[:type])
    respond_to do |format|
      format.json {render(:template => 'releases/index')}
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_product
      @product = Product.get_force_product_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def product_params
      params.require(:product).permit(:name, :description, :build_provider_id, :environment_provider_id, :test_case_provider_id,:rqm_project_alias )
    end
end
