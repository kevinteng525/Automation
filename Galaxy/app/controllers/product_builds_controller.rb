class ProductBuildsController < ApplicationController
  
  def index
    @builds = Product.get_force_builds_for_product(params[:product_id])
  end  
  
end
